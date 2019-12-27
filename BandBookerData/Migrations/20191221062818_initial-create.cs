using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BandBookerData.Migrations
{
    public partial class initialcreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Band",
                columns: table => new
                {
                    BandId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Band", x => x.BandId);
                });

            migrationBuilder.CreateTable(
                name: "Instrument",
                columns: table => new
                {
                    InstrumentId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instrument", x => x.InstrumentId);
                });

            migrationBuilder.CreateTable(
                name: "Musician",
                columns: table => new
                {
                    MusicianId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Email = table.Column<string>(maxLength: 50, nullable: false),
                    MobilePhone = table.Column<string>(maxLength: 50, nullable: true),
                    Bio = table.Column<string>(maxLength: 4000, nullable: true),
                    PhotoUrl = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Musician", x => x.MusicianId);
                });

            migrationBuilder.CreateTable(
                name: "Venue",
                columns: table => new
                {
                    VenueId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Description = table.Column<string>(nullable: true),
                    ContactName = table.Column<string>(maxLength: 50, nullable: true),
                    ContactEmail = table.Column<string>(maxLength: 50, nullable: true),
                    ContactMobilePhone = table.Column<string>(nullable: true),
                    PhotoUrl = table.Column<string>(maxLength: 255, nullable: true),
                    Address = table.Column<string>(maxLength: 50, nullable: true),
                    Address2 = table.Column<string>(maxLength: 50, nullable: true),
                    City = table.Column<string>(maxLength: 50, nullable: true),
                    State = table.Column<string>(maxLength: 50, nullable: true),
                    Zip = table.Column<string>(maxLength: 20, nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    Website = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Venue", x => x.VenueId);
                });

            migrationBuilder.CreateTable(
                name: "BandMusician",
                columns: table => new
                {
                    BandMusicianId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BandId = table.Column<int>(nullable: false),
                    MusicianId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BandMusician", x => x.BandMusicianId);
                    table.ForeignKey(
                        name: "FK_BandMusician_Band",
                        column: x => x.BandId,
                        principalTable: "Band",
                        principalColumn: "BandId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BandMusician_ToMusician",
                        column: x => x.MusicianId,
                        principalTable: "Musician",
                        principalColumn: "MusicianId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MusicianInstrument",
                columns: table => new
                {
                    MusicianInstrumentId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MusicianId = table.Column<int>(nullable: false),
                    InstrumentId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MusicianInstrument", x => x.MusicianInstrumentId);
                    table.ForeignKey(
                        name: "FK_MusicianInstrument_Instrument",
                        column: x => x.InstrumentId,
                        principalTable: "Instrument",
                        principalColumn: "InstrumentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MusicianInstrument_ToMusician",
                        column: x => x.MusicianId,
                        principalTable: "Musician",
                        principalColumn: "MusicianId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Booking",
                columns: table => new
                {
                    BookingId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Booked = table.Column<bool>(nullable: false),
                    DateBooked = table.Column<DateTime>(nullable: true),
                    VenueId = table.Column<int>(nullable: true),
                    DateAndTime = table.Column<DateTime>(nullable: false),
                    PayOffering = table.Column<string>(nullable: true),
                    BandId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Booking", x => x.BookingId);
                    table.ForeignKey(
                        name: "FK_Booking_ToBand",
                        column: x => x.BandId,
                        principalTable: "Band",
                        principalColumn: "BandId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Booking_ToVenue",
                        column: x => x.VenueId,
                        principalTable: "Venue",
                        principalColumn: "VenueId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BookingInstrument",
                columns: table => new
                {
                    BookingInstrumentId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingId = table.Column<int>(nullable: false),
                    InstrumentId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingInstrument", x => x.BookingInstrumentId);
                    table.ForeignKey(
                        name: "FK_BookingInstrument_Booking",
                        column: x => x.BookingId,
                        principalTable: "Booking",
                        principalColumn: "BookingId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BookingInstrument_Instrument",
                        column: x => x.InstrumentId,
                        principalTable: "Instrument",
                        principalColumn: "InstrumentId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BookingMusicianInstrument",
                columns: table => new
                {
                    BookingMusicianInstrumentId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingId = table.Column<int>(nullable: false),
                    MusicianId = table.Column<int>(nullable: false),
                    InstrumentId = table.Column<int>(nullable: true),
                    Response = table.Column<bool>(nullable: true),
                    ResponseDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ResponseReason = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingMusicianInstrument", x => x.BookingMusicianInstrumentId);
                    table.ForeignKey(
                        name: "FK_BookingMusicianInstrument_ToBooking",
                        column: x => x.BookingId,
                        principalTable: "Booking",
                        principalColumn: "BookingId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BookingMusicianInstrument_ToInstrument",
                        column: x => x.InstrumentId,
                        principalTable: "Instrument",
                        principalColumn: "InstrumentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BookingMusicianInstrument_ToMusician",
                        column: x => x.MusicianId,
                        principalTable: "Musician",
                        principalColumn: "MusicianId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BandMusician_BandId",
                table: "BandMusician",
                column: "BandId");

            migrationBuilder.CreateIndex(
                name: "IX_BandMusician_MusicianId",
                table: "BandMusician",
                column: "MusicianId");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_BandId",
                table: "Booking",
                column: "BandId");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_VenueId",
                table: "Booking",
                column: "VenueId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingInstrument_BookingId",
                table: "BookingInstrument",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingInstrument_InstrumentId",
                table: "BookingInstrument",
                column: "InstrumentId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingMusicianInstrument_BookingId",
                table: "BookingMusicianInstrument",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingMusicianInstrument_InstrumentId",
                table: "BookingMusicianInstrument",
                column: "InstrumentId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingMusicianInstrument_MusicianId",
                table: "BookingMusicianInstrument",
                column: "MusicianId");

            migrationBuilder.CreateIndex(
                name: "IX_MusicianInstrument_InstrumentId",
                table: "MusicianInstrument",
                column: "InstrumentId");

            migrationBuilder.CreateIndex(
                name: "IX_MusicianInstrument_MusicianId",
                table: "MusicianInstrument",
                column: "MusicianId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BandMusician");

            migrationBuilder.DropTable(
                name: "BookingInstrument");

            migrationBuilder.DropTable(
                name: "BookingMusicianInstrument");

            migrationBuilder.DropTable(
                name: "MusicianInstrument");

            migrationBuilder.DropTable(
                name: "Booking");

            migrationBuilder.DropTable(
                name: "Instrument");

            migrationBuilder.DropTable(
                name: "Musician");

            migrationBuilder.DropTable(
                name: "Band");

            migrationBuilder.DropTable(
                name: "Venue");
        }
    }
}
