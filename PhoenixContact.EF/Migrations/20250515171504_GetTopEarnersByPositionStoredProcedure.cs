using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhoenixContact.EF.Migrations
{
    /// <inheritdoc />
    public partial class GetTopEarnersByPositionStoredProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sql = @"
    CREATE OR ALTER PROCEDURE [dbo].[GetTopEarnersByPosition]
    AS
    BEGIN
        SET NOCOUNT ON;
        
        WITH RankedEmployees AS (
            SELECT 
                Id,
                FirstName,
                LastName,
                Salary,
                PositionLevel,
                Residence,
                ROW_NUMBER() OVER (PARTITION BY PositionLevel ORDER BY Salary DESC) AS RowNum
            FROM 
                [Employees]
        )
        
        SELECT 
            Id,
            FirstName,
            LastName,
            Salary,
            PositionLevel,
            Residence
        FROM 
            RankedEmployees
        WHERE 
            RowNum = 1
        ORDER BY 
            PositionLevel;
    END";

            migrationBuilder.Sql(sql);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[GetTopEarnersByPosition]");
        }
    }
}
