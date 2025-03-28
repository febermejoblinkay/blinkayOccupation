CREATE TABLE IF NOT EXISTS preprod.stays
(
    "Id" text COLLATE pg_catalog."default" NOT NULL,
    "EntryEventId" text COLLATE pg_catalog."default",
    "ExitEventId" text COLLATE pg_catalog."default",
    "EntryDate" timestamp with time zone,
    "ExitDate" timestamp with time zone,
    "InstallationId" text COLLATE pg_catalog."default",
    "ZoneId" text COLLATE pg_catalog."default" NOT NULL DEFAULT ''::text,
    "CaseId" integer,
	"InitPaymentDate" timestamp with time zone NULL,
	"EndPaymentDate" timestamp with time zone NULL,
	"InitPaymentProcessed" boolean NULL DEFAULT false,
	"EndPaymentProcessed" boolean NULL DEFAULT false,
    "Created" timestamp with time zone NOT NULL DEFAULT '-infinity'::timestamp with time zone,
    "Deleted" boolean NOT NULL DEFAULT false,
    "Updated" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_stays" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_stays_installations_InstallationId" FOREIGN KEY ("InstallationId")
        REFERENCES preprod.installations ("Id") MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE CASCADE,
    CONSTRAINT "FK_stays_parking_events_EntryEventId" FOREIGN KEY ("EntryEventId")
        REFERENCES preprod.parking_events ("Id") MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE CASCADE,
    CONSTRAINT "FK_stays_parking_events_ExitEventId" FOREIGN KEY ("ExitEventId")
        REFERENCES preprod.parking_events ("Id") MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE CASCADE,
    CONSTRAINT "FK_stays_zones_ZoneId" FOREIGN KEY ("ZoneId")
        REFERENCES preprod.zones ("Id") MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE CASCADE
)

CREATE INDEX IF NOT EXISTS "IX_stays_EntryEventId"
    ON preprod.stays USING btree
    ("EntryEventId" ASC NULLS LAST)
    TABLESPACE pg_default;
	
CREATE INDEX IF NOT EXISTS "IX_stays_ExitEventId"
    ON preprod.stays USING btree
    ("ExitEventId" ASC NULLS LAST)
    TABLESPACE pg_default;

CREATE INDEX IF NOT EXISTS "IX_stays_InstallationId"
    ON preprod.stays USING btree
    ("InstallationId" ASC NULLS LAST)
    TABLESPACE pg_default;	

CREATE INDEX IF NOT EXISTS "IX_stays_ZoneId"
    ON preprod.stays USING btree
    ("ZoneId" ASC NULLS LAST)
    TABLESPACE pg_default;

CREATE INDEX IF NOT EXISTS "IX_stays_CaseId"
    ON preprod.stays USING btree
    ("CaseId" ASC NULLS LAST)
    TABLESPACE pg_default;
	
CREATE INDEX IF NOT EXISTS "IX_stays_Updated"
    ON preprod.stays USING btree
    ("Updated" ASC NULLS LAST)
    TABLESPACE pg_default;

CREATE INDEX IF NOT EXISTS "IX_stays_InitPaymentDate"
    ON preprod.stays USING btree ("InitPaymentDate" ASC NULLS LAST);

CREATE INDEX IF NOT EXISTS "IX_stays_EndPaymentDate"
    ON preprod.stays USING btree ("EndPaymentDate" ASC NULLS LAST);

CREATE INDEX IF NOT EXISTS "IX_stays_InitPaymentProcessed"
    ON preprod.stays USING btree ("InitPaymentProcessed" ASC NULLS LAST);

CREATE INDEX IF NOT EXISTS "IX_stays_EndPaymentProcessed"
    ON preprod.stays USING btree ("EndPaymentProcessed" ASC NULLS LAST);	


---- stays_parking_rights -- 

CREATE TABLE IF NOT EXISTS preprod.stays_parking_rights
(
    "Id" text COLLATE pg_catalog."default" NOT NULL,
	"StayId" text COLLATE pg_catalog."default" NOT NULL DEFAULT ''::text,
	"ParkingRightId" text COLLATE pg_catalog."default",
	"Created" timestamp with time zone NOT NULL DEFAULT '-infinity'::timestamp with time zone,
    "Deleted" boolean NOT NULL DEFAULT false,
	"Updated" timestamp with time zone NOT NULL,
	CONSTRAINT "PK_parking_right_stays" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_parking_right_stays_StayId" FOREIGN KEY ("StayId")
        REFERENCES preprod.stays ("Id") MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE CASCADE,
    CONSTRAINT "FK_parking_rights_ParkingRightId" FOREIGN KEY ("ParkingRightId")
        REFERENCES preprod.parking_rights ("Id") MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE CASCADE
)

CREATE INDEX IF NOT EXISTS "IX_stays_parking_rights_StayId"
    ON preprod.stays_parking_rights USING btree
    ("StayId" ASC NULLS LAST)
    TABLESPACE pg_default;	
	
CREATE INDEX IF NOT EXISTS "IX_stays_parking_rights_ParkingRightId"
    ON preprod.stays_parking_rights USING btree
    ("ParkingRightId" ASC NULLS LAST)
    TABLESPACE pg_default;	
	
CREATE INDEX IF NOT EXISTS "IX_stays_parking_rights_Updated"
    ON preprod.stays_parking_rights USING btree
    ("Updated" ASC NULLS LAST)
    TABLESPACE pg_default;	

-- occupations	

CREATE TABLE IF NOT EXISTS preprod.occupations
(
    "Id" text COLLATE pg_catalog."default" NOT NULL,
	"Date" timestamp with time zone NULL,
	"InstallationId" text COLLATE pg_catalog."default" NULL,
    "ZoneId" text COLLATE pg_catalog."default" NULL,
	"TariffId" text COLLATE pg_catalog."default" NULL,
	"Paid_Real_Occupation" integer NULL DEFAULT 0,
	"Unpaid_Real_Occupation" integer NULL DEFAULT 0,
	"Paid_Occupation" integer NULL DEFAULT 0,
	"Total" integer NULL DEFAULT 0,
    "Created" timestamp with time zone NOT NULL DEFAULT '-infinity'::timestamp with time zone,
    "Deleted" boolean NOT NULL DEFAULT false,
	"Updated" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_occupations" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_occupations_installations_InstallationId" FOREIGN KEY ("InstallationId")
        REFERENCES preprod.installations ("Id") MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE CASCADE,
    CONSTRAINT "FK_occupations_zones_ZoneId" FOREIGN KEY ("ZoneId")
        REFERENCES preprod.zones ("Id") MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE CASCADE,
    CONSTRAINT "FK_occupations_tariffs_TariffId" FOREIGN KEY ("TariffId")
        REFERENCES preprod.tariffs ("Id") MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE CASCADE,		
);

CREATE INDEX IF NOT EXISTS "IX_occupations_Date"
    ON preprod.occupations USING btree
    ("Date" ASC NULLS LAST)
    TABLESPACE pg_default;	

CREATE INDEX IF NOT EXISTS "IX_occupations_InstallationId"
    ON preprod.occupations USING btree
    ("InstallationId" ASC NULLS LAST)
    TABLESPACE pg_default;	

CREATE INDEX IF NOT EXISTS "IX_occupations_ZoneId"
    ON preprod.occupations USING btree
    ("ZoneId" ASC NULLS LAST)
    TABLESPACE pg_default;
	
CREATE INDEX IF NOT EXISTS "IX_occupations_Updated"
    ON preprod.occupations USING btree
    ("Updated" ASC NULLS LAST)
    TABLESPACE pg_default;	
	
-- Agregar nueva columna a la tabla preprod.tariffs
ALTER TABLE preprod.tariffs 
ADD COLUMN "PaymentApplyAllDay" BOOLEAN DEFAULT FALSE;

-- Permitir valores nulos en la columna TariffId de la tabla preprod.capacities
ALTER TABLE preprod.capacities 
ALTER COLUMN "TariffId" DROP NOT NULL;


-- tabla para snapshots
CREATE TABLE IF NOT EXISTS preprod.occupations_snapshots
(
    "Id" text COLLATE pg_catalog."default" NOT NULL,
	"OccupationDate" timestamp with time zone NULL,
	"SnapshotDate" timestamp with time zone NULL,
	"InstallationId" text COLLATE pg_catalog."default" NULL,
    "ZoneId" text COLLATE pg_catalog."default" NULL,
	"TariffId" text COLLATE pg_catalog."default" NULL,
	"Paid_Real_Occupation" integer NULL DEFAULT 0,
	"Unpaid_Real_Occupation" integer NULL DEFAULT 0,
	"Paid_Occupation" integer NULL DEFAULT 0,
	"Total" integer NULL DEFAULT 0,
    "Created" timestamp with time zone NOT NULL DEFAULT '-infinity'::timestamp with time zone,
    "Deleted" boolean NOT NULL DEFAULT false,
	"Updated" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_occupations_snapshot" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_occupations_snapshot_installations_InstallationId" FOREIGN KEY ("InstallationId")
        REFERENCES preprod.installations ("Id") MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE CASCADE,
    CONSTRAINT "FK_occupations_snapshot_zones_ZoneId" FOREIGN KEY ("ZoneId")
        REFERENCES preprod.zones ("Id") MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE CASCADE,
    CONSTRAINT "FK_occupations_snapshot_tariffs_TariffId" FOREIGN KEY ("TariffId")
        REFERENCES preprod.tariffs ("Id") MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE CASCADE		
)

CREATE INDEX IF NOT EXISTS "IX_occupations_snapshot_SnapshotDate"
    ON preprod.occupations_snapshots USING btree
    ("SnapshotDate" ASC NULLS LAST)
    TABLESPACE pg_default;	
	
CREATE INDEX IF NOT EXISTS "IX_occupations_snapshot_OccupationDate"
    ON preprod.occupations_snapshots USING btree
    ("OccupationDate" ASC NULLS LAST)
    TABLESPACE pg_default;		

CREATE INDEX IF NOT EXISTS "IX_occupations_snapshot_InstallationId"
    ON preprod.occupations_snapshots USING btree
    ("InstallationId" ASC NULLS LAST)
    TABLESPACE pg_default;	

CREATE INDEX IF NOT EXISTS "IX_occupations_snapshot_ZoneId"
    ON preprod.occupations_snapshots USING btree
    ("ZoneId" ASC NULLS LAST)
    TABLESPACE pg_default;
	
CREATE INDEX IF NOT EXISTS "IX_occupations_snapshot_Updated"
    ON preprod.occupations_snapshots USING btree
    ("Updated" ASC NULLS LAST)
    TABLESPACE pg_default;	