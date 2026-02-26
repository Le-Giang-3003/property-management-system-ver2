USE [master]
GO
/****** Object:  Database [PropertyManagementDBVer2]    Script Date: 2/26/2026 10:52:16 AM ******/
CREATE DATABASE [PropertyManagementDBVer2]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'PropertyManagementDBVer2', FILENAME = N'D:\SQLServer\MSSQL13.MSSQLSERVER\MSSQL\DATA\PropertyManagementDBVer2.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'PropertyManagementDBVer2_log', FILENAME = N'D:\SQLServer\MSSQL13.MSSQLSERVER\MSSQL\DATA\PropertyManagementDBVer2_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO
ALTER DATABASE [PropertyManagementDBVer2] SET COMPATIBILITY_LEVEL = 130
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [PropertyManagementDBVer2].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [PropertyManagementDBVer2] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [PropertyManagementDBVer2] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [PropertyManagementDBVer2] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [PropertyManagementDBVer2] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [PropertyManagementDBVer2] SET ARITHABORT OFF 
GO
ALTER DATABASE [PropertyManagementDBVer2] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [PropertyManagementDBVer2] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [PropertyManagementDBVer2] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [PropertyManagementDBVer2] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [PropertyManagementDBVer2] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [PropertyManagementDBVer2] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [PropertyManagementDBVer2] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [PropertyManagementDBVer2] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [PropertyManagementDBVer2] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [PropertyManagementDBVer2] SET  ENABLE_BROKER 
GO
ALTER DATABASE [PropertyManagementDBVer2] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [PropertyManagementDBVer2] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [PropertyManagementDBVer2] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [PropertyManagementDBVer2] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [PropertyManagementDBVer2] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [PropertyManagementDBVer2] SET READ_COMMITTED_SNAPSHOT ON 
GO
ALTER DATABASE [PropertyManagementDBVer2] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [PropertyManagementDBVer2] SET RECOVERY FULL 
GO
ALTER DATABASE [PropertyManagementDBVer2] SET  MULTI_USER 
GO
ALTER DATABASE [PropertyManagementDBVer2] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [PropertyManagementDBVer2] SET DB_CHAINING OFF 
GO
ALTER DATABASE [PropertyManagementDBVer2] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [PropertyManagementDBVer2] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [PropertyManagementDBVer2] SET DELAYED_DURABILITY = DISABLED 
GO
EXEC sys.sp_db_vardecimal_storage_format N'PropertyManagementDBVer2', N'ON'
GO
ALTER DATABASE [PropertyManagementDBVer2] SET QUERY_STORE = OFF
GO
USE [PropertyManagementDBVer2]
GO
ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 0;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET MAXDOP = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET LEGACY_CARDINALITY_ESTIMATION = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET LEGACY_CARDINALITY_ESTIMATION = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET PARAMETER_SNIFFING = ON;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET PARAMETER_SNIFFING = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET QUERY_OPTIMIZER_HOTFIXES = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET QUERY_OPTIMIZER_HOTFIXES = PRIMARY;
GO
USE [PropertyManagementDBVer2]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 2/26/2026 10:52:16 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Bookings]    Script Date: 2/26/2026 10:52:16 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Bookings](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PropertyId] [int] NOT NULL,
	[TenantId] [int] NOT NULL,
	[Status] [int] NOT NULL,
	[ScheduledDate] [datetime2](7) NOT NULL,
	[StartTime] [time](7) NOT NULL,
	[EndTime] [time](7) NOT NULL,
	[Message] [nvarchar](1000) NULL,
	[ConfirmedAt] [datetime2](7) NULL,
	[ConfirmationNotes] [nvarchar](500) NULL,
	[CancellationReason] [nvarchar](500) NULL,
	[CancelledAt] [datetime2](7) NULL,
	[CancelledBy] [int] NULL,
	[CompletedAt] [datetime2](7) NULL,
	[CompletionNotes] [nvarchar](500) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
 CONSTRAINT [PK_Bookings] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Conversations]    Script Date: 2/26/2026 10:52:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Conversations](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[User1Id] [int] NOT NULL,
	[User2Id] [int] NOT NULL,
	[PropertyId] [int] NULL,
	[LeaseId] [int] NULL,
	[BookingId] [int] NULL,
	[Title] [nvarchar](200) NULL,
	[LastMessageAt] [datetime2](7) NOT NULL,
	[LastMessageId] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsArchived] [bit] NOT NULL,
	[UnreadCountUser1] [int] NOT NULL,
	[UnreadCountUser2] [int] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
 CONSTRAINT [PK_Conversations] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DeletedMessages]    Script Date: 2/26/2026 10:52:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DeletedMessages](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MessageId] [int] NOT NULL,
	[ConversationId] [int] NOT NULL,
	[SenderId] [int] NOT NULL,
	[DeletedBy] [int] NOT NULL,
	[OriginalContent] [nvarchar](4000) NOT NULL,
	[DeletedAt] [datetime2](7) NOT NULL,
	[Reason] [nvarchar](500) NULL,
 CONSTRAINT [PK_DeletedMessages] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Leases]    Script Date: 2/26/2026 10:52:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Leases](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PropertyId] [int] NOT NULL,
	[LandlordId] [int] NOT NULL,
	[TenantId] [int] NOT NULL,
	[RentalApplicationId] [int] NULL,
	[Status] [int] NOT NULL,
	[LeaseNumber] [nvarchar](50) NOT NULL,
	[StartDate] [datetime2](7) NOT NULL,
	[EndDate] [datetime2](7) NOT NULL,
	[MonthlyRent] [decimal](18, 2) NOT NULL,
	[DepositAmount] [decimal](18, 2) NOT NULL,
	[Currency] [nvarchar](10) NOT NULL,
	[PaymentDueDay] [int] NOT NULL,
	[LateFeePercentage] [decimal](5, 2) NULL,
	[Terms] [nvarchar](max) NULL,
	[SpecialConditions] [nvarchar](max) NULL,
	[LandlordSigned] [bit] NOT NULL,
	[LandlordSignedAt] [datetime2](7) NULL,
	[TenantSigned] [bit] NOT NULL,
	[TenantSignedAt] [datetime2](7) NULL,
	[TerminationReason] [nvarchar](1000) NULL,
	[TerminatedAt] [datetime2](7) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
 CONSTRAINT [PK_Leases] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[MaintenanceRequests]    Script Date: 2/26/2026 10:52:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MaintenanceRequests](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PropertyId] [int] NOT NULL,
	[LeaseId] [int] NOT NULL,
	[RequestedBy] [int] NOT NULL,
	[Status] [int] NOT NULL,
	[Priority] [int] NOT NULL,
	[Category] [int] NOT NULL,
	[Title] [nvarchar](300) NOT NULL,
	[Description] [nvarchar](3000) NOT NULL,
	[ImageUrls] [nvarchar](2000) NULL,
	[AssignedTo] [int] NULL,
	[AssignedAt] [datetime2](7) NULL,
	[EstimatedCost] [decimal](18, 2) NULL,
	[ActualCost] [decimal](18, 2) NULL,
	[Resolution] [nvarchar](2000) NULL,
	[ResolvedAt] [datetime2](7) NULL,
	[ScheduledDate] [datetime2](7) NULL,
	[Rating] [int] NULL,
	[Feedback] [nvarchar](1000) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
 CONSTRAINT [PK_MaintenanceRequests] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Messages]    Script Date: 2/26/2026 10:52:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Messages](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ConversationId] [int] NOT NULL,
	[SenderId] [int] NOT NULL,
	[MessageType] [int] NOT NULL,
	[Content] [nvarchar](4000) NOT NULL,
	[AttachmentUrl] [nvarchar](500) NULL,
	[AttachmentName] [nvarchar](255) NULL,
	[AttachmentMimeType] [nvarchar](100) NULL,
	[AttachmentSize] [bigint] NULL,
	[Metadata] [nvarchar](2000) NULL,
	[IsRead] [bit] NOT NULL,
	[ReadAt] [datetime2](7) NULL,
	[IsEdited] [bit] NOT NULL,
	[EditedAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NOT NULL,
	[DeletedAt] [datetime2](7) NULL,
	[ReplyToMessageId] [int] NULL,
	[SentAt] [datetime2](7) NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_Messages] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Payments]    Script Date: 2/26/2026 10:52:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Payments](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[LeaseId] [int] NOT NULL,
	[PaymentType] [int] NOT NULL,
	[Status] [int] NOT NULL,
	[PaymentMethod] [int] NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[Currency] [nvarchar](10) NOT NULL,
	[DueDate] [datetime2](7) NOT NULL,
	[PaidDate] [datetime2](7) NULL,
	[BillingMonth] [int] NULL,
	[BillingYear] [int] NULL,
	[LateFeeAmount] [decimal](18, 2) NULL,
	[Description] [nvarchar](500) NULL,
	[TransactionId] [nvarchar](100) NULL,
	[PaymentProof] [nvarchar](500) NULL,
	[Notes] [nvarchar](1000) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
 CONSTRAINT [PK_Payments] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Properties]    Script Date: 2/26/2026 10:52:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Properties](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](300) NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
	[PropertyType] [int] NOT NULL,
	[Status] [int] NOT NULL,
	[Address] [nvarchar](500) NOT NULL,
	[City] [nvarchar](100) NOT NULL,
	[District] [nvarchar](100) NOT NULL,
	[Ward] [nvarchar](100) NULL,
	[Latitude] [decimal](10, 8) NULL,
	[Longitude] [decimal](11, 8) NULL,
	[Area] [decimal](10, 2) NOT NULL,
	[Bedrooms] [int] NOT NULL,
	[Bathrooms] [int] NOT NULL,
	[Floors] [int] NULL,
	[MonthlyRent] [decimal](18, 2) NOT NULL,
	[DepositAmount] [decimal](18, 2) NOT NULL,
	[Currency] [nvarchar](10) NOT NULL,
	[Amenities] [nvarchar](2000) NULL,
	[AllowPets] [bit] NOT NULL,
	[AllowSmoking] [bit] NOT NULL,
	[MaxOccupants] [int] NULL,
	[LandlordId] [int] NOT NULL,
	[RejectionReason] [nvarchar](1000) NULL,
	[ApprovedAt] [datetime2](7) NULL,
	[ApprovedBy] [int] NULL,
	[IsAvailable] [bit] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
 CONSTRAINT [PK_Properties] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PropertyImages]    Script Date: 2/26/2026 10:52:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PropertyImages](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PropertyId] [int] NOT NULL,
	[ImageUrl] [nvarchar](500) NOT NULL,
	[Caption] [nvarchar](300) NULL,
	[IsPrimary] [bit] NOT NULL,
	[DisplayOrder] [int] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_PropertyImages] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RentalApplications]    Script Date: 2/26/2026 10:52:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RentalApplications](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PropertyId] [int] NOT NULL,
	[TenantId] [int] NOT NULL,
	[Status] [int] NOT NULL,
	[MoveInDate] [datetime2](7) NOT NULL,
	[LeaseDurationMonths] [int] NOT NULL,
	[NumberOfOccupants] [int] NOT NULL,
	[Message] [nvarchar](2000) NULL,
	[Occupation] [nvarchar](200) NULL,
	[MonthlyIncome] [decimal](18, 2) NULL,
	[EmployerName] [nvarchar](300) NULL,
	[EmployerContact] [nvarchar](100) NULL,
	[ReferenceName] [nvarchar](200) NULL,
	[ReferenceContact] [nvarchar](100) NULL,
	[ReferenceRelationship] [nvarchar](100) NULL,
	[RejectionReason] [nvarchar](1000) NULL,
	[ReviewedAt] [datetime2](7) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
 CONSTRAINT [PK_RentalApplications] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Revenues]    Script Date: 2/26/2026 10:52:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Revenues](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PropertyId] [int] NOT NULL,
	[Month] [int] NOT NULL,
	[Year] [int] NOT NULL,
	[TotalRentCollected] [decimal](18, 2) NOT NULL,
	[TotalDeposit] [decimal](18, 2) NOT NULL,
	[TotalServiceFees] [decimal](18, 2) NOT NULL,
	[TotalUtilities] [decimal](18, 2) NOT NULL,
	[TotalLateFees] [decimal](18, 2) NOT NULL,
	[TotalOtherIncome] [decimal](18, 2) NOT NULL,
	[TotalMaintenanceCost] [decimal](18, 2) NOT NULL,
	[TotalRefunds] [decimal](18, 2) NOT NULL,
	[TotalOtherExpenses] [decimal](18, 2) NOT NULL,
	[GrossRevenue] [decimal](18, 2) NOT NULL,
	[NetRevenue] [decimal](18, 2) NOT NULL,
	[OccupancyRate] [decimal](5, 2) NOT NULL,
	[DaysOccupied] [int] NOT NULL,
	[CalculatedAt] [datetime2](7) NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
 CONSTRAINT [PK_Revenues] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SystemConfigurations]    Script Date: 2/26/2026 10:52:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SystemConfigurations](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Key] [nvarchar](100) NOT NULL,
	[Value] [nvarchar](500) NOT NULL,
	[Type] [int] NOT NULL,
	[Description] [nvarchar](500) NULL,
	[Unit] [nvarchar](20) NULL,
	[IsActive] [bit] NOT NULL,
	[UpdatedBy] [int] NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
 CONSTRAINT [PK_SystemConfigurations] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TypingIndicators]    Script Date: 2/26/2026 10:52:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TypingIndicators](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ConversationId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[IsTyping] [bit] NOT NULL,
	[StartedAt] [datetime2](7) NOT NULL,
	[StoppedAt] [datetime2](7) NULL,
 CONSTRAINT [PK_TypingIndicators] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserPresences]    Script Date: 2/26/2026 10:52:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserPresences](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[Status] [int] NOT NULL,
	[ConnectionId] [nvarchar](100) NULL,
	[LastSeenAt] [datetime2](7) NULL,
	[LastActiveAt] [datetime2](7) NULL,
	[DeviceInfo] [nvarchar](500) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
 CONSTRAINT [PK_UserPresences] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Users]    Script Date: 2/26/2026 10:52:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Email] [nvarchar](256) NOT NULL,
	[PasswordHash] [nvarchar](500) NOT NULL,
	[FullName] [nvarchar](200) NOT NULL,
	[PhoneNumber] [nvarchar](20) NOT NULL,
	[Role] [int] NOT NULL,
	[Address] [nvarchar](500) NULL,
	[AvatarUrl] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL,
	[IsTenant] [bit] NOT NULL,
	[IsLandlord] [bit] NOT NULL,
	[IsEmailVerified] [bit] NOT NULL,
	[IsPhoneVerified] [bit] NOT NULL,
	[IsIdentityVerified] [bit] NOT NULL,
	[IdentityNumber] [nvarchar](20) NULL,
	[BankAccountNumber] [nvarchar](30) NULL,
	[BankName] [nvarchar](100) NULL,
	[BankAccountHolder] [nvarchar](200) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[LastLoginAt] [datetime2](7) NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Index [IX_Bookings_PropertyId]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_Bookings_PropertyId] ON [dbo].[Bookings]
(
	[PropertyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Bookings_PropertyId_ScheduledDate]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_Bookings_PropertyId_ScheduledDate] ON [dbo].[Bookings]
(
	[PropertyId] ASC,
	[ScheduledDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Bookings_ScheduledDate]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_Bookings_ScheduledDate] ON [dbo].[Bookings]
(
	[ScheduledDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Bookings_Status]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_Bookings_Status] ON [dbo].[Bookings]
(
	[Status] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Bookings_TenantId]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_Bookings_TenantId] ON [dbo].[Bookings]
(
	[TenantId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Conversations_BookingId]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_Conversations_BookingId] ON [dbo].[Conversations]
(
	[BookingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Conversations_IsActive]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_Conversations_IsActive] ON [dbo].[Conversations]
(
	[IsActive] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Conversations_LastMessageAt]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_Conversations_LastMessageAt] ON [dbo].[Conversations]
(
	[LastMessageAt] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Conversations_LeaseId]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_Conversations_LeaseId] ON [dbo].[Conversations]
(
	[LeaseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Conversations_PropertyId]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_Conversations_PropertyId] ON [dbo].[Conversations]
(
	[PropertyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Conversations_User1Id]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_Conversations_User1Id] ON [dbo].[Conversations]
(
	[User1Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Conversations_User1Id_User2Id]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_Conversations_User1Id_User2Id] ON [dbo].[Conversations]
(
	[User1Id] ASC,
	[User2Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Conversations_User2Id]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_Conversations_User2Id] ON [dbo].[Conversations]
(
	[User2Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_DeletedMessages_ConversationId]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_DeletedMessages_ConversationId] ON [dbo].[DeletedMessages]
(
	[ConversationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_DeletedMessages_DeletedAt]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_DeletedMessages_DeletedAt] ON [dbo].[DeletedMessages]
(
	[DeletedAt] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_DeletedMessages_DeletedBy]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_DeletedMessages_DeletedBy] ON [dbo].[DeletedMessages]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_DeletedMessages_MessageId]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_DeletedMessages_MessageId] ON [dbo].[DeletedMessages]
(
	[MessageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Leases_LandlordId]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_Leases_LandlordId] ON [dbo].[Leases]
(
	[LandlordId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Leases_LeaseNumber]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Leases_LeaseNumber] ON [dbo].[Leases]
(
	[LeaseNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Leases_PropertyId]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_Leases_PropertyId] ON [dbo].[Leases]
(
	[PropertyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Leases_RentalApplicationId]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Leases_RentalApplicationId] ON [dbo].[Leases]
(
	[RentalApplicationId] ASC
)
WHERE ([RentalApplicationId] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Leases_StartDate_EndDate]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_Leases_StartDate_EndDate] ON [dbo].[Leases]
(
	[StartDate] ASC,
	[EndDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Leases_Status]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_Leases_Status] ON [dbo].[Leases]
(
	[Status] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Leases_TenantId]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_Leases_TenantId] ON [dbo].[Leases]
(
	[TenantId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_MaintenanceRequests_Category]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_MaintenanceRequests_Category] ON [dbo].[MaintenanceRequests]
(
	[Category] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_MaintenanceRequests_LeaseId]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_MaintenanceRequests_LeaseId] ON [dbo].[MaintenanceRequests]
(
	[LeaseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_MaintenanceRequests_Priority]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_MaintenanceRequests_Priority] ON [dbo].[MaintenanceRequests]
(
	[Priority] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_MaintenanceRequests_PropertyId]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_MaintenanceRequests_PropertyId] ON [dbo].[MaintenanceRequests]
(
	[PropertyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_MaintenanceRequests_RequestedBy]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_MaintenanceRequests_RequestedBy] ON [dbo].[MaintenanceRequests]
(
	[RequestedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_MaintenanceRequests_Status]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_MaintenanceRequests_Status] ON [dbo].[MaintenanceRequests]
(
	[Status] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Messages_ConversationId]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_Messages_ConversationId] ON [dbo].[Messages]
(
	[ConversationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Messages_ConversationId_IsDeleted]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_Messages_ConversationId_IsDeleted] ON [dbo].[Messages]
(
	[ConversationId] ASC,
	[IsDeleted] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Messages_ConversationId_SentAt]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_Messages_ConversationId_SentAt] ON [dbo].[Messages]
(
	[ConversationId] ASC,
	[SentAt] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Messages_MessageType]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_Messages_MessageType] ON [dbo].[Messages]
(
	[MessageType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Messages_ReplyToMessageId]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_Messages_ReplyToMessageId] ON [dbo].[Messages]
(
	[ReplyToMessageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Messages_SenderId]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_Messages_SenderId] ON [dbo].[Messages]
(
	[SenderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Messages_SentAt]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_Messages_SentAt] ON [dbo].[Messages]
(
	[SentAt] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Payments_BillingYear_BillingMonth]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_Payments_BillingYear_BillingMonth] ON [dbo].[Payments]
(
	[BillingYear] ASC,
	[BillingMonth] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Payments_DueDate]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_Payments_DueDate] ON [dbo].[Payments]
(
	[DueDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Payments_LeaseId]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_Payments_LeaseId] ON [dbo].[Payments]
(
	[LeaseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Payments_Status]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_Payments_Status] ON [dbo].[Payments]
(
	[Status] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Payments_TransactionId]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_Payments_TransactionId] ON [dbo].[Payments]
(
	[TransactionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Properties_City]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_Properties_City] ON [dbo].[Properties]
(
	[City] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Properties_City_District]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_Properties_City_District] ON [dbo].[Properties]
(
	[City] ASC,
	[District] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Properties_IsAvailable]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_Properties_IsAvailable] ON [dbo].[Properties]
(
	[IsAvailable] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Properties_LandlordId]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_Properties_LandlordId] ON [dbo].[Properties]
(
	[LandlordId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Properties_MonthlyRent]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_Properties_MonthlyRent] ON [dbo].[Properties]
(
	[MonthlyRent] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Properties_PropertyType]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_Properties_PropertyType] ON [dbo].[Properties]
(
	[PropertyType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Properties_Status]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_Properties_Status] ON [dbo].[Properties]
(
	[Status] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_PropertyImages_PropertyId]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_PropertyImages_PropertyId] ON [dbo].[PropertyImages]
(
	[PropertyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_PropertyImages_PropertyId_IsPrimary]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_PropertyImages_PropertyId_IsPrimary] ON [dbo].[PropertyImages]
(
	[PropertyId] ASC,
	[IsPrimary] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_RentalApplications_PropertyId]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_RentalApplications_PropertyId] ON [dbo].[RentalApplications]
(
	[PropertyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_RentalApplications_PropertyId_TenantId_Status]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_RentalApplications_PropertyId_TenantId_Status] ON [dbo].[RentalApplications]
(
	[PropertyId] ASC,
	[TenantId] ASC,
	[Status] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_RentalApplications_Status]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_RentalApplications_Status] ON [dbo].[RentalApplications]
(
	[Status] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_RentalApplications_TenantId]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_RentalApplications_TenantId] ON [dbo].[RentalApplications]
(
	[TenantId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Revenues_PropertyId]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Revenues_PropertyId] ON [dbo].[Revenues]
(
	[PropertyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Revenues_PropertyId_Year_Month]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Revenues_PropertyId_Year_Month] ON [dbo].[Revenues]
(
	[PropertyId] ASC,
	[Year] ASC,
	[Month] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Revenues_Year_Month]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_Revenues_Year_Month] ON [dbo].[Revenues]
(
	[Year] ASC,
	[Month] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_SystemConfigurations_Key]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_SystemConfigurations_Key] ON [dbo].[SystemConfigurations]
(
	[Key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_SystemConfigurations_Type]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_SystemConfigurations_Type] ON [dbo].[SystemConfigurations]
(
	[Type] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_TypingIndicators_ConversationId_UserId]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_TypingIndicators_ConversationId_UserId] ON [dbo].[TypingIndicators]
(
	[ConversationId] ASC,
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_TypingIndicators_IsTyping]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_TypingIndicators_IsTyping] ON [dbo].[TypingIndicators]
(
	[IsTyping] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_TypingIndicators_UserId]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_TypingIndicators_UserId] ON [dbo].[TypingIndicators]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_UserPresences_ConnectionId]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_UserPresences_ConnectionId] ON [dbo].[UserPresences]
(
	[ConnectionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserPresences_LastSeenAt]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_UserPresences_LastSeenAt] ON [dbo].[UserPresences]
(
	[LastSeenAt] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserPresences_Status]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_UserPresences_Status] ON [dbo].[UserPresences]
(
	[Status] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserPresences_UserId]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_UserPresences_UserId] ON [dbo].[UserPresences]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Users_Email]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Users_Email] ON [dbo].[Users]
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Users_IsLandlord]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_Users_IsLandlord] ON [dbo].[Users]
(
	[IsLandlord] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Users_PhoneNumber]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_Users_PhoneNumber] ON [dbo].[Users]
(
	[PhoneNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Users_Role]    Script Date: 2/26/2026 10:52:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_Users_Role] ON [dbo].[Users]
(
	[Role] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Leases] ADD  DEFAULT (N'VND') FOR [Currency]
GO
ALTER TABLE [dbo].[Payments] ADD  DEFAULT (N'VND') FOR [Currency]
GO
ALTER TABLE [dbo].[Properties] ADD  DEFAULT (N'VND') FOR [Currency]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT (CONVERT([bit],(1))) FOR [IsTenant]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT (CONVERT([bit],(0))) FOR [IsLandlord]
GO
ALTER TABLE [dbo].[Bookings]  WITH CHECK ADD  CONSTRAINT [FK_Bookings_Properties_PropertyId] FOREIGN KEY([PropertyId])
REFERENCES [dbo].[Properties] ([Id])
GO
ALTER TABLE [dbo].[Bookings] CHECK CONSTRAINT [FK_Bookings_Properties_PropertyId]
GO
ALTER TABLE [dbo].[Bookings]  WITH CHECK ADD  CONSTRAINT [FK_Bookings_Users_TenantId] FOREIGN KEY([TenantId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[Bookings] CHECK CONSTRAINT [FK_Bookings_Users_TenantId]
GO
ALTER TABLE [dbo].[Conversations]  WITH CHECK ADD  CONSTRAINT [FK_Conversations_Bookings_BookingId] FOREIGN KEY([BookingId])
REFERENCES [dbo].[Bookings] ([Id])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Conversations] CHECK CONSTRAINT [FK_Conversations_Bookings_BookingId]
GO
ALTER TABLE [dbo].[Conversations]  WITH CHECK ADD  CONSTRAINT [FK_Conversations_Leases_LeaseId] FOREIGN KEY([LeaseId])
REFERENCES [dbo].[Leases] ([Id])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Conversations] CHECK CONSTRAINT [FK_Conversations_Leases_LeaseId]
GO
ALTER TABLE [dbo].[Conversations]  WITH CHECK ADD  CONSTRAINT [FK_Conversations_Properties_PropertyId] FOREIGN KEY([PropertyId])
REFERENCES [dbo].[Properties] ([Id])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Conversations] CHECK CONSTRAINT [FK_Conversations_Properties_PropertyId]
GO
ALTER TABLE [dbo].[Conversations]  WITH CHECK ADD  CONSTRAINT [FK_Conversations_Users_User1Id] FOREIGN KEY([User1Id])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[Conversations] CHECK CONSTRAINT [FK_Conversations_Users_User1Id]
GO
ALTER TABLE [dbo].[Conversations]  WITH CHECK ADD  CONSTRAINT [FK_Conversations_Users_User2Id] FOREIGN KEY([User2Id])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[Conversations] CHECK CONSTRAINT [FK_Conversations_Users_User2Id]
GO
ALTER TABLE [dbo].[Leases]  WITH CHECK ADD  CONSTRAINT [FK_Leases_Properties_PropertyId] FOREIGN KEY([PropertyId])
REFERENCES [dbo].[Properties] ([Id])
GO
ALTER TABLE [dbo].[Leases] CHECK CONSTRAINT [FK_Leases_Properties_PropertyId]
GO
ALTER TABLE [dbo].[Leases]  WITH CHECK ADD  CONSTRAINT [FK_Leases_RentalApplications_RentalApplicationId] FOREIGN KEY([RentalApplicationId])
REFERENCES [dbo].[RentalApplications] ([Id])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Leases] CHECK CONSTRAINT [FK_Leases_RentalApplications_RentalApplicationId]
GO
ALTER TABLE [dbo].[Leases]  WITH CHECK ADD  CONSTRAINT [FK_Leases_Users_LandlordId] FOREIGN KEY([LandlordId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[Leases] CHECK CONSTRAINT [FK_Leases_Users_LandlordId]
GO
ALTER TABLE [dbo].[Leases]  WITH CHECK ADD  CONSTRAINT [FK_Leases_Users_TenantId] FOREIGN KEY([TenantId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[Leases] CHECK CONSTRAINT [FK_Leases_Users_TenantId]
GO
ALTER TABLE [dbo].[MaintenanceRequests]  WITH CHECK ADD  CONSTRAINT [FK_MaintenanceRequests_Leases_LeaseId] FOREIGN KEY([LeaseId])
REFERENCES [dbo].[Leases] ([Id])
GO
ALTER TABLE [dbo].[MaintenanceRequests] CHECK CONSTRAINT [FK_MaintenanceRequests_Leases_LeaseId]
GO
ALTER TABLE [dbo].[MaintenanceRequests]  WITH CHECK ADD  CONSTRAINT [FK_MaintenanceRequests_Properties_PropertyId] FOREIGN KEY([PropertyId])
REFERENCES [dbo].[Properties] ([Id])
GO
ALTER TABLE [dbo].[MaintenanceRequests] CHECK CONSTRAINT [FK_MaintenanceRequests_Properties_PropertyId]
GO
ALTER TABLE [dbo].[MaintenanceRequests]  WITH CHECK ADD  CONSTRAINT [FK_MaintenanceRequests_Users_RequestedBy] FOREIGN KEY([RequestedBy])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[MaintenanceRequests] CHECK CONSTRAINT [FK_MaintenanceRequests_Users_RequestedBy]
GO
ALTER TABLE [dbo].[Messages]  WITH CHECK ADD  CONSTRAINT [FK_Messages_Conversations_ConversationId] FOREIGN KEY([ConversationId])
REFERENCES [dbo].[Conversations] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Messages] CHECK CONSTRAINT [FK_Messages_Conversations_ConversationId]
GO
ALTER TABLE [dbo].[Messages]  WITH CHECK ADD  CONSTRAINT [FK_Messages_Messages_ReplyToMessageId] FOREIGN KEY([ReplyToMessageId])
REFERENCES [dbo].[Messages] ([Id])
GO
ALTER TABLE [dbo].[Messages] CHECK CONSTRAINT [FK_Messages_Messages_ReplyToMessageId]
GO
ALTER TABLE [dbo].[Messages]  WITH CHECK ADD  CONSTRAINT [FK_Messages_Users_SenderId] FOREIGN KEY([SenderId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[Messages] CHECK CONSTRAINT [FK_Messages_Users_SenderId]
GO
ALTER TABLE [dbo].[Payments]  WITH CHECK ADD  CONSTRAINT [FK_Payments_Leases_LeaseId] FOREIGN KEY([LeaseId])
REFERENCES [dbo].[Leases] ([Id])
GO
ALTER TABLE [dbo].[Payments] CHECK CONSTRAINT [FK_Payments_Leases_LeaseId]
GO
ALTER TABLE [dbo].[Properties]  WITH CHECK ADD  CONSTRAINT [FK_Properties_Users_LandlordId] FOREIGN KEY([LandlordId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[Properties] CHECK CONSTRAINT [FK_Properties_Users_LandlordId]
GO
ALTER TABLE [dbo].[PropertyImages]  WITH CHECK ADD  CONSTRAINT [FK_PropertyImages_Properties_PropertyId] FOREIGN KEY([PropertyId])
REFERENCES [dbo].[Properties] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[PropertyImages] CHECK CONSTRAINT [FK_PropertyImages_Properties_PropertyId]
GO
ALTER TABLE [dbo].[RentalApplications]  WITH CHECK ADD  CONSTRAINT [FK_RentalApplications_Properties_PropertyId] FOREIGN KEY([PropertyId])
REFERENCES [dbo].[Properties] ([Id])
GO
ALTER TABLE [dbo].[RentalApplications] CHECK CONSTRAINT [FK_RentalApplications_Properties_PropertyId]
GO
ALTER TABLE [dbo].[RentalApplications]  WITH CHECK ADD  CONSTRAINT [FK_RentalApplications_Users_TenantId] FOREIGN KEY([TenantId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[RentalApplications] CHECK CONSTRAINT [FK_RentalApplications_Users_TenantId]
GO
ALTER TABLE [dbo].[Revenues]  WITH CHECK ADD  CONSTRAINT [FK_Revenues_Properties_PropertyId] FOREIGN KEY([PropertyId])
REFERENCES [dbo].[Properties] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Revenues] CHECK CONSTRAINT [FK_Revenues_Properties_PropertyId]
GO
ALTER TABLE [dbo].[TypingIndicators]  WITH CHECK ADD  CONSTRAINT [FK_TypingIndicators_Conversations_ConversationId] FOREIGN KEY([ConversationId])
REFERENCES [dbo].[Conversations] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TypingIndicators] CHECK CONSTRAINT [FK_TypingIndicators_Conversations_ConversationId]
GO
ALTER TABLE [dbo].[TypingIndicators]  WITH CHECK ADD  CONSTRAINT [FK_TypingIndicators_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TypingIndicators] CHECK CONSTRAINT [FK_TypingIndicators_Users_UserId]
GO
ALTER TABLE [dbo].[UserPresences]  WITH CHECK ADD  CONSTRAINT [FK_UserPresences_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserPresences] CHECK CONSTRAINT [FK_UserPresences_Users_UserId]
GO
USE [master]
GO
ALTER DATABASE [PropertyManagementDBVer2] SET  READ_WRITE 
GO
