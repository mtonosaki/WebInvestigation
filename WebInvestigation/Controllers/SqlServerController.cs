﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tono;
using WebInvestigation.Models;

namespace WebInvestigation.Controllers
{
    [RequireHttps]
    public class SqlServerController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Index(new SqlServerModel
            {
                ConnectionString = SqlServerModel.Default.ConnectionString,
                Sql = SqlServerModel.Default.Sql,
                SkipSql = true,
            });
        }

        [HttpPost]
        public IActionResult Index(SqlServerModel model)
        {
            var cu = ControllerUtils.From(this);
            cu.PersistInput("ConnectionString", model, SqlServerModel.Default.ConnectionString);
            cu.PersistInput("Sql", model, SqlServerModel.Default.Sql);

            if (!model.SkipSql)
            {
                try
                {
                    var sql = model.Sql.Trim();
                    if (Regex.IsMatch(sql.ToUpper(), "DROP\\s+TABLE\\s+"))
                    {
                        throw new NotSupportedException("Cannot SQL contains drop table command.");
                    }
                    if (Regex.IsMatch(sql.ToUpper(), "DELETE\\s+FROM\\s+"))
                    {
                        throw new NotSupportedException("Cannot SQL contains delete command.");
                    }
                    if (Regex.IsMatch(sql.ToUpper(), "UPDATE\\s+.+SET\\s.+"))
                    {
                        throw new NotSupportedException("Cannot SQL contains update command.");
                    }
                    if (Regex.IsMatch(sql.ToUpper(), "INSERT\\s+INTO\\s+"))
                    {
                        throw new NotSupportedException("Cannot SQL contains insert command.");
                    }

                    if (sql.StartsWith("SELECT ", StringComparison.CurrentCultureIgnoreCase))
                    {
                        // remove input TOP(*) word
                        var mstr = Regex.Match(sql, "\\s+[Tt][Oo][Pp]\\s*\\(\\d+\\)\\s+");
                        if(mstr.Success)
                        {
                            sql = sql.Replace(mstr.Value, " ");
                        }
                        sql = "SELECT TOP(20) " + StrUtil.MidSkip(sql, "SELECT\\s+");
                    }
                    model.LatestSql = sql;

                    using var con = new SqlConnection(model.ConnectionString);
                    con.Open();
                    using var sqlcommand = new SqlCommand(sql, con);
                    using var row = sqlcommand.ExecuteReader();

                    var cs = row.GetColumnSchema();
                    model.Table = new List<List<string>>();
                    model.Table.Add(cs.Select(a => a.ColumnName).ToList());
                    model.Table.Add(cs.Select(a => $"<{a.DataTypeName}>").ToList());

                    while (row.Read())
                    {
                        var r = new List<string>();
                        for (var col = 0; col < row.FieldCount; col++)
                        {
                            r.Add(Tono.DbUtil.ToString(row[col], "(null)"));
                        }
                        model.Table.Add(r);
                        if (model.Table.Count > 22)
                        {
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    model.ErrorMessage = ex.Message;
                }
            }

            if (model.Table == null)
            {
                model.Table = new List<List<string>>();
            }
            while (model.Table.Count < 2)
            {
                model.Table.Add(new List<string>());
            }

            model.SkipSql = false;
            return View(model);
        }
    }
}
