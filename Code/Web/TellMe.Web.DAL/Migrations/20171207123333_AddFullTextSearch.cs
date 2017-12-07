using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace TellMe.Web.DAL.Migrations
{
    public partial class AddFullTextSearch : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE FULLTEXT CATALOG [fullTextCatalog] WITH ACCENT_SENSITIVITY = ON
            CREATE FULLTEXT INDEX ON AspNetUsers(FullName, UserName) KEY INDEX PK_AspNetUsers ON [fullTextCatalog]
            CREATE FULLTEXT INDEX ON Story(Title) KEY INDEX PK_Story ON [fullTextCatalog]
            CREATE FULLTEXT INDEX ON Event(Title, Description) KEY INDEX PK_Event ON [fullTextCatalog]", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            DROP FULLTEXT INDEX ON AspNetUsers
            DROP FULLTEXT INDEX ON Story
            DROP FULLTEXT INDEX ON [Event]
            DROP FULLTEXT CATALOG [fullTextCatalog]", true);
        }
    }
}