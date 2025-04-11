CREATE TABLE IF NOT EXISTS prod.stays
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
        REFERENCES prod.installations ("Id") MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE CASCADE,
    CONSTRAINT "FK_stays_parking_events_EntryEventId" FOREIGN KEY ("EntryEventId")
        REFERENCES prod.parking_events ("Id") MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE CASCADE,
    CONSTRAINT "FK_stays_parking_events_ExitEventId" FOREIGN KEY ("ExitEventId")
        REFERENCES prod.parking_events ("Id") MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE CASCADE,
    CONSTRAINT "FK_stays_zones_ZoneId" FOREIGN KEY ("ZoneId")
        REFERENCES prod.zones ("Id") MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE CASCADE
)

CREATE INDEX IF NOT EXISTS "IX_stays_EntryEventId"
    ON prod.stays USING btree
    ("EntryEventId" ASC NULLS LAST)
    TABLESPACE pg_default;
	
CREATE INDEX IF NOT EXISTS "IX_stays_ExitEventId"
    ON prod.stays USING btree
    ("ExitEventId" ASC NULLS LAST)
    TABLESPACE pg_default;

CREATE INDEX IF NOT EXISTS "IX_stays_InstallationId"
    ON prod.stays USING btree
    ("InstallationId" ASC NULLS LAST)
    TABLESPACE pg_default;	

CREATE INDEX IF NOT EXISTS "IX_stays_ZoneId"
    ON prod.stays USING btree
    ("ZoneId" ASC NULLS LAST)
    TABLESPACE pg_default;

CREATE INDEX IF NOT EXISTS "IX_stays_CaseId"
    ON prod.stays USING btree
    ("CaseId" ASC NULLS LAST)
    TABLESPACE pg_default;
	
CREATE INDEX IF NOT EXISTS "IX_stays_Updated"
    ON prod.stays USING btree
    ("Updated" ASC NULLS LAST)
    TABLESPACE pg_default;

CREATE INDEX IF NOT EXISTS "IX_stays_InitPaymentDate"
    ON prod.stays USING btree ("InitPaymentDate" ASC NULLS LAST);

CREATE INDEX IF NOT EXISTS "IX_stays_EndPaymentDate"
    ON prod.stays USING btree ("EndPaymentDate" ASC NULLS LAST);

CREATE INDEX IF NOT EXISTS "IX_stays_InitPaymentProcessed"
    ON prod.stays USING btree ("InitPaymentProcessed" ASC NULLS LAST);

CREATE INDEX IF NOT EXISTS "IX_stays_EndPaymentProcessed"
    ON prod.stays USING btree ("EndPaymentProcessed" ASC NULLS LAST);	


---- stays_parking_rights -- 

CREATE TABLE IF NOT EXISTS prod.stays_parking_rights
(
    "Id" text COLLATE pg_catalog."default" NOT NULL,
	"StayId" text COLLATE pg_catalog."default" NOT NULL DEFAULT ''::text,
	"ParkingRightId" text COLLATE pg_catalog."default",
	"Created" timestamp with time zone NOT NULL DEFAULT '-infinity'::timestamp with time zone,
    "Deleted" boolean NOT NULL DEFAULT false,
	"Updated" timestamp with time zone NOT NULL,
	CONSTRAINT "PK_parking_right_stays" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_parking_right_stays_StayId" FOREIGN KEY ("StayId")
        REFERENCES prod.stays ("Id") MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE CASCADE,
    CONSTRAINT "FK_parking_rights_ParkingRightId" FOREIGN KEY ("ParkingRightId")
        REFERENCES prod.parking_rights ("Id") MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE CASCADE
)

CREATE INDEX IF NOT EXISTS "IX_stays_parking_rights_StayId"
    ON prod.stays_parking_rights USING btree
    ("StayId" ASC NULLS LAST)
    TABLESPACE pg_default;	
	
CREATE INDEX IF NOT EXISTS "IX_stays_parking_rights_ParkingRightId"
    ON prod.stays_parking_rights USING btree
    ("ParkingRightId" ASC NULLS LAST)
    TABLESPACE pg_default;	
	
CREATE INDEX IF NOT EXISTS "IX_stays_parking_rights_Updated"
    ON prod.stays_parking_rights USING btree
    ("Updated" ASC NULLS LAST)
    TABLESPACE pg_default;	

-- occupations	

CREATE TABLE IF NOT EXISTS prod.occupations
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
        REFERENCES prod.installations ("Id") MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE CASCADE,
    CONSTRAINT "FK_occupations_zones_ZoneId" FOREIGN KEY ("ZoneId")
        REFERENCES prod.zones ("Id") MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE CASCADE,
    CONSTRAINT "FK_occupations_tariffs_TariffId" FOREIGN KEY ("TariffId")
        REFERENCES prod.tariffs ("Id") MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE CASCADE		
)

CREATE INDEX IF NOT EXISTS "IX_occupations_Date"
    ON prod.occupations USING btree
    ("Date" ASC NULLS LAST)
    TABLESPACE pg_default;	

CREATE INDEX IF NOT EXISTS "IX_occupations_InstallationId"
    ON prod.occupations USING btree
    ("InstallationId" ASC NULLS LAST)
    TABLESPACE pg_default;	

CREATE INDEX IF NOT EXISTS "IX_occupations_ZoneId"
    ON prod.occupations USING btree
    ("ZoneId" ASC NULLS LAST)
    TABLESPACE pg_default;
	
CREATE INDEX IF NOT EXISTS "IX_occupations_Updated"
    ON prod.occupations USING btree
    ("Updated" ASC NULLS LAST)
    TABLESPACE pg_default;	
	
-- 1. Solo uno permitido por combinación cuando TariffId IS NULL
CREATE UNIQUE INDEX IF NOT EXISTS uq_occupations_null_tariff
    ON prod.occupations ("Date", "InstallationId", "ZoneId")
    WHERE "TariffId" IS NULL;

-- 2. Solo uno permitido por combinación cuando ZoneId IS NULL (pero TariffId no lo es)
CREATE UNIQUE INDEX IF NOT EXISTS uq_occupations_null_zone
    ON prod.occupations ("Date", "InstallationId", "TariffId")
    WHERE "ZoneId" IS NULL AND "TariffId" IS NOT NULL;

-- 3. General para cuando ambos tienen valor
CREATE UNIQUE INDEX IF NOT EXISTS uq_occupations_full
    ON prod.occupations ("Date", "InstallationId", "ZoneId", "TariffId")
    WHERE "ZoneId" IS NOT NULL AND "TariffId" IS NOT NULL;	
	
-- Agregar nueva columna a la tabla prod.tariffs
ALTER TABLE prod.tariffs 
ADD COLUMN "PaymentApplyAllDay" BOOLEAN DEFAULT FALSE;

-- Permitir valores nulos en la columna TariffId de la tabla prod.capacities
ALTER TABLE prod.capacities 
ALTER COLUMN "TariffId" DROP NOT NULL;


-- tabla para snapshots
CREATE TABLE IF NOT EXISTS prod.occupations_snapshots
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
        REFERENCES prod.installations ("Id") MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE CASCADE,
    CONSTRAINT "FK_occupations_snapshot_zones_ZoneId" FOREIGN KEY ("ZoneId")
        REFERENCES prod.zones ("Id") MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE CASCADE,
    CONSTRAINT "FK_occupations_snapshot_tariffs_TariffId" FOREIGN KEY ("TariffId")
        REFERENCES prod.tariffs ("Id") MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE CASCADE		
)

CREATE INDEX IF NOT EXISTS "IX_occupations_snapshot_SnapshotDate"
    ON prod.occupations_snapshots USING btree
    ("SnapshotDate" ASC NULLS LAST)
    TABLESPACE pg_default;	
	
CREATE INDEX IF NOT EXISTS "IX_occupations_snapshot_OccupationDate"
    ON prod.occupations_snapshots USING btree
    ("OccupationDate" ASC NULLS LAST)
    TABLESPACE pg_default;		

CREATE INDEX IF NOT EXISTS "IX_occupations_snapshot_InstallationId"
    ON prod.occupations_snapshots USING btree
    ("InstallationId" ASC NULLS LAST)
    TABLESPACE pg_default;	

CREATE INDEX IF NOT EXISTS "IX_occupations_snapshot_ZoneId"
    ON prod.occupations_snapshots USING btree
    ("ZoneId" ASC NULLS LAST)
    TABLESPACE pg_default;
	
CREATE INDEX IF NOT EXISTS "IX_occupations_snapshot_Updated"
    ON prod.occupations_snapshots USING btree
    ("Updated" ASC NULLS LAST)
    TABLESPACE pg_default;	
	
	
-- New tariffs in PRE for Marlins --

INSERT INTO prod.tariffs(
	"Id", "Updated", "InstallationId", "Name", "VehicleMakes")
VALUES
('110901', NOW(), '110009', 'Event Rate', '{}'),
('110902', NOW(), '110009', 'Whitelist Marlins', '{}'),
('110903', NOW(), '110009', 'On Demand - Premium Parking Rate', '{}'),
('110904', NOW(), '110009', 'Reservation- Premium Parking Rate', '{}'),
('110905', NOW(), '110009', 'Validations - Premium Parking Rate', '{}'),
('110906', NOW(), '110009', 'Staff - Premium Parking Rate', '{}'),
('110907', NOW(), '110009', 'MLB Ballpark - Premium Parking Rate', '{}');	