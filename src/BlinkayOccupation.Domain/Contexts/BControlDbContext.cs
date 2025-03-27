using System;
using System.Collections.Generic;
using BlinkayOccupation.Domain.Interceptors;
using BlinkayOccupation.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace BlinkayOccupation.Domain.Contexts;

public partial class BControlDbContext : DbContext
{
    private readonly IConfiguration _configuration;
    private readonly AuditInterceptor _auditInterceptor = new();

    public BControlDbContext(DbContextOptions<BControlDbContext> options, IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (_configuration != null)
        {
            optionsBuilder.UseNpgsql(_configuration.GetConnectionString("bControlDb"));
        }
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory) // Directorio donde corre el proceso
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
                //.AddEnvironmentVariables()
                .Build();

            var connectionString = config.GetConnectionString("bControlDb");
            optionsBuilder.UseNpgsql(connectionString);
        }

        optionsBuilder.AddInterceptors(_auditInterceptor);

        string environmentVariable = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        if (environmentVariable == "pre-usa")
        {
            optionsBuilder.LogTo(Console.WriteLine);
            optionsBuilder.EnableSensitiveDataLogging(true);
        }
    }

    public virtual DbSet<Attachments> Attachments { get; set; }

    public virtual DbSet<Blobs> Blobs { get; set; }

    public virtual DbSet<Capacities> Capacities { get; set; }

    public virtual DbSet<ConsolidatedOccupation> ConsolidatedOccupation { get; set; }

    public virtual DbSet<DataProtectionKeys> DataProtectionKeys { get; set; }

    public virtual DbSet<InputDevices> InputDevices { get; set; }

    public virtual DbSet<Installations> Installations { get; set; }

    public virtual DbSet<OccupancyStatus> OccupancyStatus { get; set; }

    public virtual DbSet<OccupancyStatusByZones> OccupancyStatusByZones { get; set; }

    public virtual DbSet<OccupationEvents> OccupationEvents { get; set; }

    public virtual DbSet<Occupations> Occupations { get; set; }

    public virtual DbSet<ParkingEvents> ParkingEvents { get; set; }

    public virtual DbSet<ParkingRights> ParkingRights { get; set; }

    public virtual DbSet<Shapes> Shapes { get; set; }

    public virtual DbSet<Spaces> Spaces { get; set; }

    public virtual DbSet<Stays> Stays { get; set; }

    public virtual DbSet<StaysParkingRights> StaysParkingRights { get; set; }

    public virtual DbSet<StreetSections> StreetSections { get; set; }

    public virtual DbSet<Streets> Streets { get; set; }

    public virtual DbSet<Tariffs> Tariffs { get; set; }

    public virtual DbSet<Users> Users { get; set; }

    public virtual DbSet<VehicleEvents> VehicleEvents { get; set; }

    public virtual DbSet<VwParkingRights> VwParkingRights { get; set; }

    public virtual DbSet<Zones> Zones { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("fuzzystrmatch");

        //modelBuilder.Ignore<Attachments>();
        //modelBuilder.Ignore<Blobs>();
        //modelBuilder.Ignore<Capacities>();
        //modelBuilder.Ignore<ConsolidatedOccupation>();
        //modelBuilder.Ignore<DataProtectionKeys>();
        //modelBuilder.Ignore<Installations>();
        //modelBuilder.Ignore<OccupancyStatus>();
        //modelBuilder.Ignore<OccupancyStatusByZones>();
        //modelBuilder.Ignore<OccupationEvents>();
        //modelBuilder.Ignore<ParkingEvents>();
        //modelBuilder.Ignore<Shapes>();
        //modelBuilder.Ignore<Spaces>();
        //modelBuilder.Ignore<StreetSections>();
        //modelBuilder.Ignore<Streets>();
        //modelBuilder.Ignore<Tariffs>();
        //modelBuilder.Ignore<Users>();
        //modelBuilder.Ignore<VehicleEvents>();
        //modelBuilder.Ignore<VwParkingRights>();
        //modelBuilder.Ignore<Zones>();


        modelBuilder.Entity<Attachments>(entity =>
        {
            entity.ToTable("attachments", "preprod");

            entity.HasIndex(e => e.Created, "IX_attachments_Created");

            entity.HasIndex(e => e.DeviceId, "IX_attachments_DeviceId");

            entity.HasIndex(e => e.Direction, "IX_attachments_Direction");

            entity.HasIndex(e => e.InstallationId, "IX_attachments_InstallationId");

            entity.Property(e => e.Direction).HasDefaultValue(0);

            entity.HasOne(d => d.Device).WithMany(p => p.Attachments).HasForeignKey(d => d.DeviceId);

            entity.HasOne(d => d.Installation).WithMany(p => p.Attachments).HasForeignKey(d => d.InstallationId);
        });

        modelBuilder.Entity<Blobs>(entity =>
        {
            entity.ToTable("blobs", "preprod");

            entity.HasIndex(e => e.Created, "IX_blobs_Created");
        });

        modelBuilder.Entity<Capacities>(entity =>
        {
            entity.ToTable("capacities", "preprod");

            entity.HasIndex(e => e.InstallationId, "IX_capacities_InstallationId");

            entity.HasIndex(e => e.SpaceId, "IX_capacities_SpaceId");

            entity.HasIndex(e => e.StreetSectionId, "IX_capacities_StreetSectionId");

            entity.HasIndex(e => e.TariffId, "IX_capacities_TariffId");

            entity.HasIndex(e => e.Updated, "IX_capacities_Updated");

            entity.HasIndex(e => e.ValidFrom, "IX_capacities_ValidFrom");

            entity.HasIndex(e => e.ValidTo, "IX_capacities_ValidTo");

            entity.HasIndex(e => e.ZoneId, "IX_capacities_ZoneId");

            entity.Property(e => e.Created).HasDefaultValueSql("'-infinity'::timestamp with time zone");
            entity.Property(e => e.TariffId).HasDefaultValueSql("''::text");

            entity.HasOne(d => d.Installation).WithMany(p => p.Capacities).HasForeignKey(d => d.InstallationId);

            entity.HasOne(d => d.Space).WithMany(p => p.Capacities).HasForeignKey(d => d.SpaceId);

            entity.HasOne(d => d.StreetSection).WithMany(p => p.Capacities).HasForeignKey(d => d.StreetSectionId);

            entity.HasOne(d => d.Tariff).WithMany(p => p.Capacities).HasForeignKey(d => d.TariffId);

            entity.HasOne(d => d.Zone).WithMany(p => p.Capacities).HasForeignKey(d => d.ZoneId);
        });

        modelBuilder.Entity<ConsolidatedOccupation>(entity =>
        {
            entity.ToTable("consolidated_occupation", "preprod");

            entity.HasIndex(e => e.Date, "IX_consolidated_occupation_Date");

            entity.Property(e => e.InstallationId).HasColumnName("Installation_Id");
            entity.Property(e => e.InstallationValue).HasColumnName("Installation_Value");
            entity.Property(e => e.SectionId).HasColumnName("Section_Id");
            entity.Property(e => e.SectionValue).HasColumnName("Section_Value");
            entity.Property(e => e.TariffId).HasColumnName("Tariff_Id");
            entity.Property(e => e.TariffValue).HasColumnName("Tariff_Value");
            entity.Property(e => e.ZoneId).HasColumnName("Zone_Id");
            entity.Property(e => e.ZoneValue).HasColumnName("Zone_Value");
        });

        modelBuilder.Entity<DataProtectionKeys>(entity =>
        {
            entity.ToTable("DataProtectionKeys", "preprod");
        });

        modelBuilder.Entity<InputDevices>(entity =>
        {
            entity.ToTable("input_devices", "preprod");

            entity.HasIndex(e => e.ConfigurationLastUpdated, "IX_input_devices_Configuration_LastUpdated");

            entity.HasIndex(e => e.InstallationId, "IX_input_devices_InstallationId");

            entity.HasIndex(e => e.LocationLatitude, "IX_input_devices_Location_Latitude");

            entity.HasIndex(e => e.LocationLongitude, "IX_input_devices_Location_Longitude");

            entity.HasIndex(e => e.StatusLastUpdated, "IX_input_devices_Status_LastUpdated");

            entity.Property(e => e.CameraType).HasDefaultValue(0);
            entity.Property(e => e.ConfigurationLastUpdated).HasColumnName("Configuration_LastUpdated");
            entity.Property(e => e.ConfigurationValue).HasColumnName("Configuration_Value");
            entity.Property(e => e.LastReceivedEvent).HasDefaultValueSql("'-infinity'::timestamp with time zone");
            entity.Property(e => e.LocationLatitude).HasColumnName("Location_Latitude");
            entity.Property(e => e.LocationLongitude).HasColumnName("Location_Longitude");
            entity.Property(e => e.ParkingAreas).HasColumnType("jsonb");
            entity.Property(e => e.StatusLastUpdated).HasColumnName("Status_LastUpdated");
            entity.Property(e => e.StatusMetadata).HasColumnName("Status_Metadata");
            entity.Property(e => e.StatusStatus).HasColumnName("Status_Status");

            entity.HasOne(d => d.Installation).WithMany(p => p.InputDevices).HasForeignKey(d => d.InstallationId);
        });

        modelBuilder.Entity<Installations>(entity =>
        {
            entity.ToTable("installations", "preprod");

            entity.HasIndex(e => e.ExternalId, "IX_installations_ExternalId");

            entity.HasIndex(e => e.Updated, "IX_installations_Updated");

            entity.Property(e => e.ConfigurationEnterGracePeriod).HasColumnName("Configuration_EnterGracePeriod");
            entity.Property(e => e.ConfigurationEventMatchingSpan).HasColumnName("Configuration_EventMatchingSpan");
            entity.Property(e => e.ConfigurationMaxParkingEventDuration).HasColumnName("Configuration_MaxParkingEventDuration");
            entity.Property(e => e.ConfigurationParkingRightMatchOffset).HasColumnName("Configuration_ParkingRightMatchOffset");
            entity.Property(e => e.ConfigurationParkingType)
                .HasDefaultValue(0)
                .HasColumnName("Configuration_ParkingType");
            entity.Property(e => e.ConfigurationTimeZoneId).HasColumnName("Configuration_TimeZoneId");
            entity.Property(e => e.Created).HasDefaultValueSql("'-infinity'::timestamp with time zone");
            entity.Property(e => e.LanguageId).HasDefaultValueSql("''::text");
            entity.Property(e => e.LastConsolidation).HasDefaultValueSql("'-infinity'::timestamp with time zone");
            entity.Property(e => e.Name).HasDefaultValueSql("''::text");
        });

        modelBuilder.Entity<OccupancyStatus>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("occupancy_status", "preprod");

            entity.Property(e => e.Attachmentsstorageid).HasColumnName("attachmentsstorageid");
            entity.Property(e => e.ConfigurationEnterGracePeriod).HasColumnName("Configuration_EnterGracePeriod");
            entity.Property(e => e.ConfigurationTimeZoneId).HasColumnName("Configuration_TimeZoneId");
            entity.Property(e => e.Installationid).HasColumnName("installationid");
            entity.Property(e => e.LocationLatitude).HasColumnName("Location_Latitude");
            entity.Property(e => e.LocationLongitude).HasColumnName("Location_Longitude");
            entity.Property(e => e.Peid).HasColumnName("peid");
            entity.Property(e => e.Prid).HasColumnName("prid");
            entity.Property(e => e.Ssectionid).HasColumnName("ssectionid");
            entity.Property(e => e.Utcnow).HasColumnName("utcnow");
            entity.Property(e => e.Utcoffset).HasColumnName("utcoffset");
            entity.Property(e => e.Zoneid).HasColumnName("zoneid");
        });

        modelBuilder.Entity<OccupancyStatusByZones>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("occupancy_status_by_zones", "preprod");

            entity.Property(e => e.Attachmentsstoragedatetime).HasColumnName("attachmentsstoragedatetime");
            entity.Property(e => e.Attachmentsstorageid).HasColumnName("attachmentsstorageid");
            entity.Property(e => e.Capacity).HasColumnName("capacity");
            entity.Property(e => e.Device).HasColumnName("device");
            entity.Property(e => e.LocationLatitude).HasColumnName("Location_Latitude");
            entity.Property(e => e.LocationLongitude).HasColumnName("Location_Longitude");
            entity.Property(e => e.Occupancy).HasColumnName("occupancy");
        });

        modelBuilder.Entity<OccupationEvents>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("occupation_events", "preprod");

            entity.Property(e => e.EnterAttachments).HasColumnName("enter_attachments");
            entity.Property(e => e.EnterVehicleColor).HasColumnName("enter_vehicle_color");
            entity.Property(e => e.EnterVehicleConfidenceColor).HasColumnName("enter_vehicle_confidence_color");
            entity.Property(e => e.EnterVehicleConfidenceMake).HasColumnName("enter_vehicle_confidence_make");
            entity.Property(e => e.EnterVehicleConfidenceModel).HasColumnName("enter_vehicle_confidence_model");
            entity.Property(e => e.EnterVehicleConfidenceNationality).HasColumnName("enter_vehicle_confidence_nationality");
            entity.Property(e => e.EnterVehicleConfidencePlate).HasColumnName("enter_vehicle_confidence_plate");
            entity.Property(e => e.EnterVehicleConfidenceType).HasColumnName("enter_vehicle_confidence_type");
            entity.Property(e => e.EnterVehicleMake).HasColumnName("enter_vehicle_make");
            entity.Property(e => e.EnterVehicleModel).HasColumnName("enter_vehicle_model");
            entity.Property(e => e.EnterVehicleNationality).HasColumnName("enter_vehicle_nationality");
            entity.Property(e => e.EnterVehiclePlate).HasColumnName("enter_vehicle_plate");
            entity.Property(e => e.EnterVehicleType).HasColumnName("enter_vehicle_type");
            entity.Property(e => e.ExitAttachments).HasColumnName("exit_attachments");
            entity.Property(e => e.ExitVehicleColor).HasColumnName("exit_vehicle_color");
            entity.Property(e => e.ExitVehicleConfidenceColor).HasColumnName("exit_vehicle_confidence_color");
            entity.Property(e => e.ExitVehicleConfidenceMake).HasColumnName("exit_vehicle_confidence_make");
            entity.Property(e => e.ExitVehicleConfidenceModel).HasColumnName("exit_vehicle_confidence_model");
            entity.Property(e => e.ExitVehicleConfidenceNationality).HasColumnName("exit_vehicle_confidence_nationality");
            entity.Property(e => e.ExitVehicleConfidencePlate).HasColumnName("exit_vehicle_confidence_plate");
            entity.Property(e => e.ExitVehicleConfidenceType).HasColumnName("exit_vehicle_confidence_type");
            entity.Property(e => e.ExitVehicleMake).HasColumnName("exit_vehicle_make");
            entity.Property(e => e.ExitVehicleModel).HasColumnName("exit_vehicle_model");
            entity.Property(e => e.ExitVehicleNationality).HasColumnName("exit_vehicle_nationality");
            entity.Property(e => e.ExitVehiclePlate).HasColumnName("exit_vehicle_plate");
            entity.Property(e => e.ExitVehicleType).HasColumnName("exit_vehicle_type");
            entity.Property(e => e.Expiredtolerance).HasColumnName("expiredtolerance");
            entity.Property(e => e.Peid).HasColumnName("peid");
            entity.Property(e => e.Prcreated).HasColumnName("prcreated");
            entity.Property(e => e.Prid).HasColumnName("prid");
            entity.Property(e => e.Unpaidtolerance).HasColumnName("unpaidtolerance");
            entity.Property(e => e.Updatelocal)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatelocal");
            entity.Property(e => e.Utcnow).HasColumnName("utcnow");
            entity.Property(e => e.Utcoffset).HasColumnName("utcoffset");
        });

        modelBuilder.Entity<Occupations>(entity =>
        {
            entity.ToTable("occupations", "preprod");

            entity.HasIndex(e => e.Date, "IX_occupations_Date");

            entity.HasIndex(e => e.InstallationId, "IX_occupations_InstallationId");

            entity.HasIndex(e => e.Updated, "IX_occupations_Updated");

            entity.HasIndex(e => e.ZoneId, "IX_occupations_ZoneId");

            entity.Property(e => e.Created).HasDefaultValueSql("'-infinity'::timestamp with time zone");
            entity.Property(e => e.Deleted).HasDefaultValue(false);
            entity.Property(e => e.PaidOccupation)
                .HasDefaultValue(0)
                .HasColumnName("Paid_Occupation");
            entity.Property(e => e.PaidRealOccupation)
                .HasDefaultValue(0)
                .HasColumnName("Paid_Real_Occupation");
            entity.Property(e => e.Total).HasDefaultValue(0);
            entity.Property(e => e.UnpaidRealOccupation)
                .HasDefaultValue(0)
                .HasColumnName("Unpaid_Real_Occupation");

            entity.HasOne(d => d.Installation).WithMany(p => p.Occupations)
                .HasForeignKey(d => d.InstallationId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.Tariff).WithMany(p => p.Occupations)
                .HasForeignKey(d => d.TariffId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.Zone).WithMany(p => p.Occupations)
                .HasForeignKey(d => d.ZoneId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ParkingEvents>(entity =>
        {
            entity.ToTable("parking_events", "preprod");

            entity.HasIndex(e => e.Enter, "IX_parking_events_Enter");

            entity.HasIndex(e => e.Exit, "IX_parking_events_Exit");

            entity.HasIndex(e => e.InstallationId, "IX_parking_events_InstallationId");

            entity.HasIndex(e => e.ParkingRightId, "IX_parking_events_ParkingRightId");

            entity.HasIndex(e => e.SpaceId, "IX_parking_events_SpaceId");

            entity.HasIndex(e => e.StreetSectionId, "IX_parking_events_StreetSectionId");

            entity.HasIndex(e => e.TariffId, "IX_parking_events_TariffId");

            entity.HasIndex(e => e.Type, "IX_parking_events_Type");

            entity.HasIndex(e => e.Updated, "IX_parking_events_Updated");

            entity.HasIndex(e => e.ZoneId, "IX_parking_events_ZoneId");

            entity.Property(e => e.ClosingReason).HasDefaultValue(0);
            entity.Property(e => e.Created).HasDefaultValueSql("'-infinity'::timestamp with time zone");
            entity.Property(e => e.DeviceId).HasDefaultValueSql("''::text");
            entity.Property(e => e.TariffId).HasDefaultValueSql("''::text");
            entity.Property(e => e.Type).HasDefaultValue(0);
            entity.Property(e => e.ZoneId).HasDefaultValueSql("''::text");

            entity.HasOne(d => d.Installation).WithMany(p => p.ParkingEvents).HasForeignKey(d => d.InstallationId);

            entity.HasOne(d => d.ParkingRight).WithMany(p => p.ParkingEvents).HasForeignKey(d => d.ParkingRightId);

            entity.HasOne(d => d.Space).WithMany(p => p.ParkingEvents).HasForeignKey(d => d.SpaceId);

            entity.HasOne(d => d.StreetSection).WithMany(p => p.ParkingEvents).HasForeignKey(d => d.StreetSectionId);

            entity.HasOne(d => d.Tariff).WithMany(p => p.ParkingEvents).HasForeignKey(d => d.TariffId);

            entity.HasOne(d => d.Zone).WithMany(p => p.ParkingEvents).HasForeignKey(d => d.ZoneId);
        });

        modelBuilder.Entity<ParkingRights>(entity =>
        {
            entity.ToTable("parking_rights", "preprod");

            entity.HasIndex(e => e.Deleted, "IX_parking_rights_Deleted");

            entity.HasIndex(e => e.ExternalId, "IX_parking_rights_ExternalId");

            entity.HasIndex(e => e.InstallationId, "IX_parking_rights_InstallationId");

            entity.HasIndex(e => e.Plates, "IX_parking_rights_Plates");

            entity.HasIndex(e => e.SpaceId, "IX_parking_rights_SpaceId");

            entity.HasIndex(e => e.State, "IX_parking_rights_State");

            entity.HasIndex(e => e.StreetSectionId, "IX_parking_rights_StreetSectionId");

            entity.HasIndex(e => e.TariffId, "IX_parking_rights_TariffId");

            entity.HasIndex(e => e.Updated, "IX_parking_rights_Updated");

            entity.HasIndex(e => e.ValidFrom, "IX_parking_rights_ValidFrom");

            entity.HasIndex(e => e.ValidTo, "IX_parking_rights_ValidTo");

            entity.HasIndex(e => e.ZoneId, "IX_parking_rights_ZoneId");

            entity.Property(e => e.Deleted).HasDefaultValue(false);
            entity.Property(e => e.ExternalId).HasDefaultValueSql("''::text");
            entity.Property(e => e.State).HasDefaultValue(0);
            entity.Property(e => e.TariffId).HasDefaultValueSql("''::text");
            entity.Property(e => e.ValidTo).HasDefaultValueSql("'-infinity'::timestamp with time zone");
            entity.Property(e => e.ZoneId).HasDefaultValueSql("''::text");

            entity.HasOne(d => d.Installation).WithMany(p => p.ParkingRights).HasForeignKey(d => d.InstallationId);

            entity.HasOne(d => d.Space).WithMany(p => p.ParkingRights).HasForeignKey(d => d.SpaceId);

            entity.HasOne(d => d.StreetSection).WithMany(p => p.ParkingRights).HasForeignKey(d => d.StreetSectionId);

            entity.HasOne(d => d.Tariff).WithMany(p => p.ParkingRights).HasForeignKey(d => d.TariffId);

            entity.HasOne(d => d.Zone).WithMany(p => p.ParkingRights).HasForeignKey(d => d.ZoneId);
        });

        modelBuilder.Entity<Shapes>(entity =>
        {
            entity.ToTable("shapes", "preprod");

            entity.HasIndex(e => e.ExternalId, "IX_shapes_ExternalId");

            entity.HasIndex(e => e.Updated, "IX_shapes_Updated");

            entity.HasIndex(e => e.ValidFrom, "IX_shapes_ValidFrom");

            entity.HasIndex(e => e.ValidTo, "IX_shapes_ValidTo");

            entity.Property(e => e.Created).HasDefaultValueSql("'-infinity'::timestamp with time zone");
            entity.Property(e => e.ValidTo).HasDefaultValueSql("'-infinity'::timestamp with time zone");
        });

        modelBuilder.Entity<Spaces>(entity =>
       {
           entity.ToTable("spaces", "preprod");

           entity.HasIndex(e => e.BoundingBoxBottom, "IX_spaces_BoundingBox_Bottom");

           entity.HasIndex(e => e.BoundingBoxLeft, "IX_spaces_BoundingBox_Left");

           entity.HasIndex(e => e.BoundingBoxRight, "IX_spaces_BoundingBox_Right");

           entity.HasIndex(e => e.BoundingBoxTop, "IX_spaces_BoundingBox_Top");

           entity.HasIndex(e => e.ExternalId, "IX_spaces_ExternalId");

           entity.HasIndex(e => e.Index, "IX_spaces_Index");

           entity.HasIndex(e => e.InstallationId, "IX_spaces_InstallationId");

           entity.HasIndex(e => e.LocationLatitude, "IX_spaces_Location_Latitude");

           entity.HasIndex(e => e.LocationLongitude, "IX_spaces_Location_Longitude");

           entity.HasIndex(e => e.ShapeId, "IX_spaces_ShapeId").IsUnique();

           entity.HasIndex(e => e.StreetSectionId, "IX_spaces_StreetSectionId");

           entity.HasIndex(e => e.Updated, "IX_spaces_Updated");

           entity.HasIndex(e => e.ValidFrom, "IX_spaces_ValidFrom");

           entity.HasIndex(e => e.ValidTo, "IX_spaces_ValidTo");

           entity.HasIndex(e => e.ZoneId, "IX_spaces_ZoneId");

           entity.Property(e => e.BoundingBoxBottom).HasColumnName("BoundingBox_Bottom");
           entity.Property(e => e.BoundingBoxLeft).HasColumnName("BoundingBox_Left");
           entity.Property(e => e.BoundingBoxRight).HasColumnName("BoundingBox_Right");
           entity.Property(e => e.BoundingBoxTop).HasColumnName("BoundingBox_Top");
           entity.Property(e => e.Created).HasDefaultValueSql("'-infinity'::timestamp with time zone");
           entity.Property(e => e.ExternalId).HasDefaultValueSql("''::text");
           entity.Property(e => e.LocationLatitude).HasColumnName("Location_Latitude");
           entity.Property(e => e.LocationLongitude).HasColumnName("Location_Longitude");
           entity.Property(e => e.ValidTo).HasDefaultValueSql("'-infinity'::timestamp with time zone");
           entity.Property(e => e.ZoneId).HasDefaultValueSql("''::text");

           entity.HasOne(d => d.Installation).WithMany(p => p.Spaces).HasForeignKey(d => d.InstallationId);

           entity.HasOne(d => d.Shape).WithOne(p => p.Spaces).HasForeignKey<Spaces>(d => d.ShapeId);

           entity.HasOne(d => d.StreetSection).WithMany(p => p.Spaces).HasForeignKey(d => d.StreetSectionId);

           entity.HasOne(d => d.Zone).WithMany(p => p.Spaces).HasForeignKey(d => d.ZoneId);
       });

        modelBuilder.Entity<Stays>(entity =>
        {
            entity.ToTable("stays", "preprod");

            entity.HasIndex(e => e.CaseId, "IX_stays_CaseId");

            entity.HasIndex(e => e.EntryEventId, "IX_stays_EntryEventId");

            entity.HasIndex(e => e.ExitEventId, "IX_stays_ExitEventId");

            entity.HasIndex(e => e.InstallationId, "IX_stays_InstallationId");

            entity.HasIndex(e => e.Updated, "IX_stays_Updated");

            entity.HasIndex(e => e.ZoneId, "IX_stays_ZoneId");

            entity.Property(e => e.Created).HasDefaultValueSql("'-infinity'::timestamp with time zone");
            entity.Property(e => e.Deleted).HasDefaultValue(false);
            entity.Property(e => e.ZoneId).HasDefaultValueSql("''::text");

            entity.HasOne(d => d.EntryEvent).WithMany(p => p.StaysEntryEvent)
                .HasForeignKey(d => d.EntryEventId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.ExitEvent).WithMany(p => p.StaysExitEvent)
                .HasForeignKey(d => d.ExitEventId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.Installation).WithMany(p => p.Stays)
                .HasForeignKey(d => d.InstallationId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.Zone).WithMany(p => p.Stays).HasForeignKey(d => d.ZoneId);
        });

        modelBuilder.Entity<StaysParkingRights>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_parking_right_stays");

            entity.ToTable("stays_parking_rights", "preprod");

            entity.HasIndex(e => e.ParkingRightId, "IX_stays_parking_rights_ParkingRightId");

            entity.HasIndex(e => e.StayId, "IX_stays_parking_rights_StayId");

            entity.HasIndex(e => e.Updated, "IX_stays_parking_rights_Updated");

            entity.Property(e => e.Created).HasDefaultValueSql("'-infinity'::timestamp with time zone");
            entity.Property(e => e.Deleted).HasDefaultValue(false);
            entity.Property(e => e.StayId).HasDefaultValueSql("''::text");

            entity.HasOne(d => d.ParkingRight).WithMany(p => p.StaysParkingRights)
                .HasForeignKey(d => d.ParkingRightId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_parking_rights_ParkingRightId");

            entity.HasOne(d => d.Stay).WithMany(p => p.StaysParkingRights)
                .HasForeignKey(d => d.StayId)
                .HasConstraintName("FK_parking_right_stays_StayId");
        });

        modelBuilder.Entity<StreetSections>(entity =>
        {
            entity.ToTable("street_sections", "preprod");

            entity.HasIndex(e => e.BoundingBoxBottom, "IX_street_sections_BoundingBox_Bottom");

            entity.HasIndex(e => e.BoundingBoxLeft, "IX_street_sections_BoundingBox_Left");

            entity.HasIndex(e => e.BoundingBoxRight, "IX_street_sections_BoundingBox_Right");

            entity.HasIndex(e => e.BoundingBoxTop, "IX_street_sections_BoundingBox_Top");

            entity.HasIndex(e => e.ExternalId, "IX_street_sections_ExternalId");

            entity.HasIndex(e => e.Index, "IX_street_sections_Index");

            entity.HasIndex(e => e.InstallationId, "IX_street_sections_InstallationId");

            entity.HasIndex(e => e.LocationLatitude, "IX_street_sections_Location_Latitude");

            entity.HasIndex(e => e.LocationLongitude, "IX_street_sections_Location_Longitude");

            entity.HasIndex(e => e.ShapeId, "IX_street_sections_ShapeId").IsUnique();

            entity.HasIndex(e => e.StreetId, "IX_street_sections_StreetId");

            entity.HasIndex(e => e.Updated, "IX_street_sections_Updated");

            entity.HasIndex(e => e.ValidFrom, "IX_street_sections_ValidFrom");

            entity.HasIndex(e => e.ValidTo, "IX_street_sections_ValidTo");

            entity.HasIndex(e => e.ZoneId, "IX_street_sections_ZoneId");

            entity.Property(e => e.BoundingBoxBottom).HasColumnName("BoundingBox_Bottom");
            entity.Property(e => e.BoundingBoxLeft).HasColumnName("BoundingBox_Left");
            entity.Property(e => e.BoundingBoxRight).HasColumnName("BoundingBox_Right");
            entity.Property(e => e.BoundingBoxTop).HasColumnName("BoundingBox_Top");
            entity.Property(e => e.Created).HasDefaultValueSql("'-infinity'::timestamp with time zone");
            entity.Property(e => e.ExternalId).HasDefaultValueSql("''::text");
            entity.Property(e => e.LocationLatitude).HasColumnName("Location_Latitude");
            entity.Property(e => e.LocationLongitude).HasColumnName("Location_Longitude");
            entity.Property(e => e.StreetId).HasDefaultValueSql("''::text");
            entity.Property(e => e.ValidTo).HasDefaultValueSql("'-infinity'::timestamp with time zone");

            entity.HasOne(d => d.Installation).WithMany(p => p.StreetSections).HasForeignKey(d => d.InstallationId);

            entity.HasOne(d => d.Shape).WithOne(p => p.StreetSections).HasForeignKey<StreetSections>(d => d.ShapeId);

            entity.HasOne(d => d.Street).WithMany(p => p.StreetSections).HasForeignKey(d => d.StreetId);

            entity.HasOne(d => d.Zone).WithMany(p => p.StreetSections).HasForeignKey(d => d.ZoneId);
        });

        modelBuilder.Entity<Streets>(entity =>
        {
            entity.ToTable("streets", "preprod");

            entity.HasIndex(e => e.ExternalId, "IX_streets_ExternalId");

            entity.HasIndex(e => e.Index, "IX_streets_Index");

            entity.HasIndex(e => e.InstallationId, "IX_streets_InstallationId");

            entity.HasIndex(e => e.Updated, "IX_streets_Updated");

            entity.HasIndex(e => e.ValidFrom, "IX_streets_ValidFrom");

            entity.HasIndex(e => e.ValidTo, "IX_streets_ValidTo");

            entity.Property(e => e.Created).HasDefaultValueSql("'-infinity'::timestamp with time zone");
            entity.Property(e => e.ValidTo).HasDefaultValueSql("'-infinity'::timestamp with time zone");

            entity.HasOne(d => d.Installation).WithMany(p => p.Streets).HasForeignKey(d => d.InstallationId);
        });

        modelBuilder.Entity<Tariffs>(entity =>
        {
            entity.ToTable("tariffs", "preprod");

            entity.HasIndex(e => e.InstallationId, "IX_tariffs_InstallationId");

            entity.HasIndex(e => e.Updated, "IX_tariffs_Updated");

            entity.Property(e => e.Created).HasDefaultValueSql("'-infinity'::timestamp with time zone");
            entity.Property(e => e.IsDefault).HasDefaultValue(false);
            entity.Property(e => e.IsResident).HasDefaultValue(false);

            entity.HasOne(d => d.Installation).WithMany(p => p.Tariffs).HasForeignKey(d => d.InstallationId);
        });

        modelBuilder.Entity<Users>(entity =>
        {
            entity.ToTable("users", "preprod");
        });

        modelBuilder.Entity<VehicleEvents>(entity =>
        {
            entity.ToTable("vehicle_events", "preprod");

            entity.HasIndex(e => e.Created, "IX_vehicle_events_Created");

            entity.HasIndex(e => e.Direction, "IX_vehicle_events_Direction");

            entity.HasIndex(e => e.InstallationId, "IX_vehicle_events_InstallationId");

            entity.HasIndex(e => e.ParkingEventId, "IX_vehicle_events_ParkingEventId");

            entity.HasIndex(e => e.Plate, "IX_vehicle_events_Plate");

            entity.HasIndex(e => e.SpaceId, "IX_vehicle_events_SpaceId");

            entity.HasIndex(e => e.StreetSectionId, "IX_vehicle_events_StreetSectionId");

            entity.HasIndex(e => e.ZoneId, "IX_vehicle_events_ZoneId");

            entity.Property(e => e.ConfidenceColor).HasColumnName("Confidence_Color");
            entity.Property(e => e.ConfidenceMake).HasColumnName("Confidence_Make");
            entity.Property(e => e.ConfidenceModel).HasColumnName("Confidence_Model");
            entity.Property(e => e.ConfidenceNationality).HasColumnName("Confidence_Nationality");
            entity.Property(e => e.ConfidencePlate).HasColumnName("Confidence_Plate");
            entity.Property(e => e.ConfidenceType).HasColumnName("Confidence_Type");
            entity.Property(e => e.DeviceDate).HasColumnName("Device_Date");
            entity.Property(e => e.DeviceId).HasColumnName("Device_Id");
            entity.Property(e => e.DeviceInserted).HasColumnName("Device_Inserted");

            entity.HasOne(d => d.Installation).WithMany(p => p.VehicleEvents).HasForeignKey(d => d.InstallationId);

            entity.HasOne(d => d.ParkingEvent).WithMany(p => p.VehicleEvents).HasForeignKey(d => d.ParkingEventId);

            entity.HasOne(d => d.Space).WithMany(p => p.VehicleEvents).HasForeignKey(d => d.SpaceId);

            entity.HasOne(d => d.StreetSection).WithMany(p => p.VehicleEvents).HasForeignKey(d => d.StreetSectionId);

            entity.HasOne(d => d.Zone).WithMany(p => p.VehicleEvents).HasForeignKey(d => d.ZoneId);
        });

        modelBuilder.Entity<VwParkingRights>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_parking_rights", "preprod");
        });

        modelBuilder.Entity<Zones>(entity =>
        {
            entity.ToTable("zones", "preprod");

            entity.HasIndex(e => e.BoundingBoxBottom, "IX_zones_BoundingBox_Bottom");

            entity.HasIndex(e => e.BoundingBoxLeft, "IX_zones_BoundingBox_Left");

            entity.HasIndex(e => e.BoundingBoxRight, "IX_zones_BoundingBox_Right");

            entity.HasIndex(e => e.BoundingBoxTop, "IX_zones_BoundingBox_Top");

            entity.HasIndex(e => e.ExternalId, "IX_zones_ExternalId");

            entity.HasIndex(e => e.Index, "IX_zones_Index");

            entity.HasIndex(e => e.InstallationId, "IX_zones_InstallationId");

            entity.HasIndex(e => e.LocationLatitude, "IX_zones_Location_Latitude");

            entity.HasIndex(e => e.LocationLongitude, "IX_zones_Location_Longitude");

            entity.HasIndex(e => e.ShapeId, "IX_zones_ShapeId").IsUnique();

            entity.HasIndex(e => e.Updated, "IX_zones_Updated");

            entity.HasIndex(e => e.ValidFrom, "IX_zones_ValidFrom");

            entity.HasIndex(e => e.ValidTo, "IX_zones_ValidTo");

            entity.Property(e => e.BoundingBoxBottom).HasColumnName("BoundingBox_Bottom");
            entity.Property(e => e.BoundingBoxLeft).HasColumnName("BoundingBox_Left");
            entity.Property(e => e.BoundingBoxRight).HasColumnName("BoundingBox_Right");
            entity.Property(e => e.BoundingBoxTop).HasColumnName("BoundingBox_Top");
            entity.Property(e => e.Created).HasDefaultValueSql("'-infinity'::timestamp with time zone");
            entity.Property(e => e.ExternalId).HasDefaultValueSql("''::text");
            entity.Property(e => e.LocationLatitude).HasColumnName("Location_Latitude");
            entity.Property(e => e.LocationLongitude).HasColumnName("Location_Longitude");
            entity.Property(e => e.ValidTo).HasDefaultValueSql("'-infinity'::timestamp with time zone");

            entity.HasOne(d => d.Installation).WithMany(p => p.Zones).HasForeignKey(d => d.InstallationId);

            entity.HasOne(d => d.Shape).WithOne(p => p.Zones).HasForeignKey<Zones>(d => d.ShapeId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
