
GO
-- MediaType Table
-- This table stores the different media types (e.g., images, videos) that can be associated with trip activities or costs.
CREATE TABLE [MediaType](
   [MediaType] INT IDENTITY,  -- Unique identifier for each media type
   [Description] VARCHAR(50) NOT NULL,  -- Description of the media type ("Image", "Video")
   [CreatedAt] DATETIME,
   [UpdatedAt] DATETIME,
   CONSTRAINT [PK_MediaType] PRIMARY KEY([MediaType]),   -- MediaType is the primary key
   CONSTRAINT [UQ_MediaType_Description] UNIQUE ([Description]) -- Unique descrition
   
);
ALTER TABLE [MediaType] ADD  CONSTRAINT [DF_MediaType_CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt];
ALTER TABLE [MediaType] ADD  CONSTRAINT [DF_MediaType_UpdatedAt]  DEFAULT (getdate()) FOR [UpdatedAt];
GO
-- ActivityType Table
-- This table stores the different types of activities (e.g., sightseeing, adventure) that can be planned during a trip.
CREATE TABLE [ActivityType](
   [ActivityTypeID] INT IDENTITY,  -- Unique identifier for each activity type
   [Description] VARCHAR(50) UNIQUE NOT NULL,
   [CreatedAt] DATETIME ,
   [UpdatedAt] DATETIME,
   CONSTRAINT [PK_ActivityType] PRIMARY KEY([ActivityTypeID]),   -- ActivityTypeID is the primary key
   CONSTRAINT [UQ_ActivityType_Description] UNIQUE ([Description])

);
ALTER TABLE [ActivityType] ADD  CONSTRAINT [DF_ActivityType_CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt];
ALTER TABLE [ActivityType] ADD  CONSTRAINT [DF_ActivityType_UpdatedAt]  DEFAULT (getdate()) FOR [UpdatedAt];
GO
-- Currency Table
-- This table stores information about various currencies used in the travel planning system.
CREATE TABLE [Currency](
   [CurrencyCode] VARCHAR(3),  -- Currency code (e.g., "USD", "EUR")
   [Name] VARCHAR(50) NOT NULL,  -- Name of the currency (e.g., "US Dollar", "Euro")
   [Symbol] VARCHAR(5) NOT NULL,  -- Symbol for the currency (e.g., "$", "€")
   [ExchangeRate] DECIMAL(10,4),  -- The exchange rate for converting this currency to a base currency
   [CreatedAt] DATETIME ,
   [UpdatedAt] DATETIME,
   CONSTRAINT [PK_Currency] PRIMARY KEY([CurrencyCode])  -- CurrencyCode is the primary key
   
);
ALTER TABLE [Currency] ADD  CONSTRAINT [DF_Currency_CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt];
ALTER TABLE [Currency] ADD  CONSTRAINT [DF_Currency_UpdatedAt]  DEFAULT (getdate()) FOR [UpdatedAt];
GO
-- Trip Table
-- This table stores the main details about each trip, including the name, description, dates, and budget.
CREATE TABLE [Trip](
   [TripID] INT IDENTITY,  -- Unique identifier for each trip
   [Name] VARCHAR(50),  -- Name of the trip
   [Description] VARCHAR(MAX) NOT NULL,  -- Detailed description of the trip
   [StartDate] DATE NOT NULL,  -- Start date of the trip
   [EndDate] DATE NOT NULL,  -- End date of the trip
   [Budget] MONEY NOT NULL,  -- Budget allocated for the trip
   [IsActive] BIT,  -- Indicates if the trip is currently active
   [NumberPeople] INT NOT NULL,  -- Number of people participating in the trip
   [TripBackgroundGUID] [uniqueidentifier] NULL,  -- Path to an image or file representing the trip background
   [CurrencyCode] VARCHAR(3) NOT NULL,  -- Currency used for the trip, linked to the Currency table
   [CreatedAt] DATETIME,
   [UpdatedAt] DATETIME,
   CONSTRAINT [PK_Trip] PRIMARY KEY([TripID]),  -- TripID is the primary key
   CONSTRAINT [FK_Trip_CurrencyCode] FOREIGN KEY([CurrencyCode]) REFERENCES [Currency]([CurrencyCode]),  -- Foreign key to the Currency table
   CONSTRAINT [CHK_Trip_EndDate_After_StartDate] CHECK ([EndDate] > [StartDate]),
   CONSTRAINT [CHK_Trip_Budget_Has_Positive] check ([Budget]>=0)
  
);
ALTER TABLE [Trip] ADD  CONSTRAINT [DF_Trip_CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt];
ALTER TABLE [Trip] ADD  CONSTRAINT [DF_Trip_UpdatedAt]  DEFAULT (getdate()) FOR [UpdatedAt];
ALTER TABLE [Trip] ADD CONSTRAINT [DF_Trip_IsActive] DEFAULT ((1)) FOR [IsActive];
GO
-- Activity Table
-- This table stores the details of the activities planned for each trip, including name, description, sequence, and cost.
CREATE TABLE [Activity](
   [TripID] INT,  -- Identifier for the trip this activity is part of  
   [Name] VARCHAR(50) NOT NULL,  -- Name of the activity
   [Description] VARCHAR(MAX),  -- Description of the activity
   [Sequence] INT NOT NULL,  -- Sequence order of the activity within the trip
   [GoogleLink] VARCHAR(MAX),  -- Link to additional information about the activity (e.g., Google Maps)
   [PlannedCost] MONEY,  -- Estimated cost for the activity 
   [ActivityTypeID] INT NOT NULL,  -- Type of activity, linked to the ActivityType table
   [CreatedAt] DATETIME,
   [UpdatedAt] DATETIME,
   [ActivityID] [int] IDENTITY(1,1) NOT NULL,
   [ActivityDate] [date] NULL,
   CONSTRAINT [PK_Activity]			 PRIMARY KEY([TripID], [ActivityID]),  -- Composite primary key ([TripID], [ActivityID])
   CONSTRAINT [FK_Activity_Trip]	FOREIGN KEY([TripID]) REFERENCES [Trip]([TripID]),  -- Foreign key to the Trip table
   CONSTRAINT [FK_Activity_ActivityType] FOREIGN KEY([ActivityTypeID]) REFERENCES [ActivityType]([ActivityTypeID]),  -- Foreign key to the ActivityType table
   CONSTRAINT [UQ_Activity_Sequence] UNIQUE ([TripID], [Sequence]), -- Ensure the sequence not contains 2 same value by Trip ID
  
);
ALTER TABLE [Activity] ADD  CONSTRAINT [DF_Activity_CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt];
ALTER TABLE [Activity] ADD  CONSTRAINT [DF_Activity_UpdatedAt]  DEFAULT (getdate()) FOR [UpdatedAt];
ALTER TABLE [Activity] ADD  CONSTRAINT [DF_Activity_PlannedCost]  DEFAULT (0) FOR [PlannedCost];
GO
-- LogBook Table
-- This table stores logs for each trip and activity, tracking changes and updates.
CREATE TABLE [LogBook](
   [LogBookID] INT IDENTITY,  -- Unique identifier for each log entry
   [Description] VARCHAR(MAX) NOT NULL,  -- Description of the log entry   
   [TripLogBook] INT,  -- Trip associated with this log entry, linked to the Trip table
   [TripID] INT,  -- Trip identifier, linked to the Trip table
   [ActivityID] INT,  -- Activity identifier, linked to the Activity table
   [CreatedAt] DATETIME,
   [UpdatedAt] DATETIME,
   CONSTRAINT [PK_LogBook]				PRIMARY KEY([LogBookID]),  -- LogBookID is the primary key
   CONSTRAINT [FK_LogBook_Trip]			FOREIGN KEY([TripLogBook]) REFERENCES [Trip]([TripID]),  -- Foreign key to the Trip table
   CONSTRAINT [FK_LogBook_Activity]		FOREIGN KEY([TripID], [ActivityID]) REFERENCES [Activity]([TripID], [ActivityID])  -- Foreign key to the Activity table

);
ALTER TABLE [LogBook] ADD  CONSTRAINT [DF_LogBook_CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt];
ALTER TABLE [LogBook] ADD  CONSTRAINT [DF_LogBook_UpdatedAt]  DEFAULT (getdate()) FOR [UpdatedAt];
GO
-- ActivityCost Table
-- This table stores the costs associated with activities, including price and currency.
CREATE TABLE [ActivityCost](
   [ActivityCostID] INT IDENTITY,  -- Unique identifier for each activity cost
   [Name] VARCHAR(50),  -- Name of the cost item (e.g., "Entrance Fee", "Guide Fee")
   [Price] MONEY NOT NULL,  -- Cost of the activity
   [CurrencyCode] VARCHAR(3) NOT NULL,  -- Currency code for the cost, linked to the Currency table
   [TripID] INT NOT NULL,  -- Trip the cost is associated with
   [ActivityID] INT NOT NULL,  -- Activity the cost is associated with
   [CreatedAt] DATETIME,
   [UpdatedAt] DATETIME,
   CONSTRAINT PK_ActivityCost PRIMARY KEY([ActivityCostID]),  -- ActivityCostID is the primary key
   CONSTRAINT [FK_ActivityCost_Currency] FOREIGN KEY([CurrencyCode]) REFERENCES [Currency]([CurrencyCode]),  -- Foreign key to the Currency table
   CONSTRAINT [FK_ActivityCost_Activity] FOREIGN KEY([TripID], [ActivityID]) REFERENCES [Activity]([TripID], [ActivityID]),  -- Foreign key to the Activity table
   CONSTRAINT [CHK_ActivityCost_Price] CHECK ([Price]>=0)

);
ALTER TABLE [ActivityCost] ADD  CONSTRAINT [DF_ActivityCost_CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt];
ALTER TABLE [ActivityCost] ADD  CONSTRAINT [DF_ActivityCost_UpdatedAt]  DEFAULT (getdate()) FOR [UpdatedAt];
GO
-- Attendee Table
-- This table stores information about attendees of activities, including their names and email addresses.
CREATE TABLE [Attendee](
   [AttendeeID] INT IDENTITY,  -- Unique identifier for each attendee
   [Name] VARCHAR(50) NOT NULL,  -- First name of the attendee
   [LastName] VARCHAR(50),  -- Last name of the attendee
   [TripID] INT NOT NULL,  -- Trip the attendee is part of
   [ActivityID] INT NOT NULL,  -- Activity the attendee is participating in
   [CreatedAt] DATETIME ,
   [UpdatedAt] DATETIME,
   CONSTRAINT [PK_Attendee] PRIMARY KEY([AttendeeID]),  -- AttendeeID is the primary key
   CONSTRAINT [FK_Attendee_Activity] FOREIGN KEY([TripID], [ActivityID]) REFERENCES [Activity]([TripID], [ActivityID])  -- Foreign key to the Activity table

);
ALTER TABLE [Attendee] ADD  CONSTRAINT [DF_Attendee_CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt];
ALTER TABLE [Attendee] ADD  CONSTRAINT [DF_Attendee_UpdatedAt]  DEFAULT (getdate()) FOR [UpdatedAt];
GO
-- Media Table
-- This table stores media files (e.g., images, videos) related to activities and costs.
CREATE TABLE [Media](
   [MediaID] INT IDENTITY,  -- Unique identifier for each media file
   [FileGUID] [uniqueidentifier] NOT NULL,
   [Description] varchar(MAX) NOT NULL,  -- Description of the media file (e.g., "Trip photo")
   [UploadedAt] DATETIME,  -- Timestamp when the media file was uploaded
   [ActivityCostID] INT,  -- Activity cost associated with this media
   [MediaType] INT NOT NULL,  -- Media type (e.g., image, video), linked to the MediaType table
   [TripID] INT,  -- Trip associated with this media
   [CreatedAt] DATETIME ,
   [UpdatedAt] DATETIME,
   CONSTRAINT PK_Media PRIMARY KEY([MediaID]),  -- MediaID is the primary key
   CONSTRAINT [FK_Media_ActivityCost] FOREIGN KEY([ActivityCostID]) REFERENCES [ActivityCost]([ActivityCostID]),  -- Foreign key to the ActivityCost table
   CONSTRAINT [FK_Media_MediaType] FOREIGN KEY([MediaType]) REFERENCES [MediaType]([MediaType]),  -- Foreign key to the MediaType table
   CONSTRAINT [FK_Media_Trip] FOREIGN KEY([TripID]) REFERENCES [Trip]([TripID])  -- Foreign key to the Trip table

);
ALTER TABLE [Media] ADD  CONSTRAINT [DF_Media_CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt];
ALTER TABLE [Media] ADD  CONSTRAINT [DF_Media_UpdatedAt]  DEFAULT (getdate()) FOR [UpdatedAt];
ALTER TABLE [Media] ADD  CONSTRAINT [DF_Media_UploadedAt]  DEFAULT (getdate()) FOR [UploadedAt];
ALTER TABLE [Media] ADD  CONSTRAINT [UQ_FileGUID] unique([FileGUID]);
GO
