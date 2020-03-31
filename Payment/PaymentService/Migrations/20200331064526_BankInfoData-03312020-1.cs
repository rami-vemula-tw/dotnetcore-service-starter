using System;
using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PaymentService.Migrations
{
    public partial class BankInfoData033120201 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sqlFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Migrations/SQLScripts/BankInfoData-03312020-1.sql"); 
            migrationBuilder.Sql(File.ReadAllText(sqlFile));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
