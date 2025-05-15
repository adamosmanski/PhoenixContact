using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhoenixContact.EF.Migrations
{
    /// <inheritdoc />
    public partial class GetLowestGrossEarnersByCity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sql = @"CREATE OR ALTER PROCEDURE [dbo].[GetLowestGrossEarnersByCity]
AS
BEGIN
    SET NOCOUNT ON;
    
    WITH EmployeesWithGrossSalary AS (
        SELECT 
            Id,
            FirstName,
            LastName,
            Salary,
            Salary + (Salary * 0.19) AS GrossSalary, 
            PositionLevel,
            Residence,
            ROW_NUMBER() OVER (PARTITION BY Residence ORDER BY Salary * 0.19 ASC) AS RowNum
        FROM 
            [dbo].[Employees]
        WHERE
            Residence IS NOT NULL
    )
    
    SELECT 
        Id,
        FirstName,
        LastName,
        Salary,
        GrossSalary,
        PositionLevel,
        Residence
    FROM 
        EmployeesWithGrossSalary
    WHERE 
        RowNum = 1
    ORDER BY 
        Residence;
END";

            migrationBuilder.Sql(sql);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[GetLowestGrossEarnersByCity]");
        }
    }
}
