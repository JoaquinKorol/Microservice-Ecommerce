﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payment.Migrations
{
    /// <inheritdoc />
    public partial class ChangeMercadoPagoIdToLong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "MercadoPagoPaymentId",
                table: "Payments",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "MercadoPagoPaymentId",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");
        }
    }
}
