using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Transport.Data.Migrations
{
    public partial class RemoveKorisnickoIme : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropColumn(
                name: "KorisnickoIme",
                table: "Korisnik");

            migrationBuilder.AddColumn<bool>(
                name: "PrimaEmail",
                table: "Korisnik",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrimaEmail",
                table: "Korisnik");

            migrationBuilder.AddColumn<string>(
                name: "KorisnickoIme",
                table: "Korisnik",
                type: "char(100)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "UK_Korisnik",
                table: "Korisnik",
                column: "KorisnickoIme",
                unique: true);
        }
    }
}
