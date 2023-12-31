USE [master]
GO
/****** Object:  Database [OrderService]    Script Date: 2020/9/9 16:03:49 ******/
CREATE DATABASE [OrderService]
 CONTAINMENT = NONE
GO
ALTER DATABASE [OrderService] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [OrderService].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [OrderService] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [OrderService] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [OrderService] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [OrderService] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [OrderService] SET ARITHABORT OFF 
GO
ALTER DATABASE [OrderService] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [OrderService] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [OrderService] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [OrderService] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [OrderService] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [OrderService] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [OrderService] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [OrderService] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [OrderService] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [OrderService] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [OrderService] SET  DISABLE_BROKER 
GO
ALTER DATABASE [OrderService] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [OrderService] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [OrderService] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [OrderService] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [OrderService] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [OrderService] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [OrderService] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [OrderService] SET RECOVERY FULL 
GO
ALTER DATABASE [OrderService] SET  MULTI_USER 
GO
ALTER DATABASE [OrderService] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [OrderService] SET DB_CHAINING OFF 
GO
ALTER DATABASE [OrderService] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [OrderService] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
EXEC sys.sp_db_vardecimal_storage_format N'OrderService', N'ON'
GO
USE [OrderService]
GO
/****** Object:  Table [dbo].[Company]    Script Date: 2020/9/9 16:03:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Company](
	[Id] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[Name] [nvarchar](500) NOT NULL,
	[CreateTime] [datetime] NOT NULL,
	[CreatorId] [int] NOT NULL,
	[LastModifierId] [int] NULL,
	[LastModifyTime] [datetime] NULL,
 CONSTRAINT [PK_Company] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[User]    Script Date: 2020/9/9 16:03:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[User](
	[Id] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[Name] [nvarchar](50) NULL,
	[Account] [varchar](100) NOT NULL,
	[Password] [varchar](100) NOT NULL,
	[Email] [varchar](200) NULL,
	[Mobile] [varchar](30) NULL,
	[CompanyId] [int] NOT NULL,
	[CompanyName] [nvarchar](500) NULL,
	[State] [int] NOT NULL,
	[UserType] [int] NOT NULL,
	[LastLoginTime] [datetime] NOT NULL,
	[CreateTime] [datetime] NOT NULL,
	[CreatorId] [int] NOT NULL,
	[LastModifierId] [int] NOT NULL,
	[LastModifyTime] [datetime] NOT NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'用户状态  0正常 1冻结 2删除' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'User', @level2type=N'COLUMN',@level2name=N'State'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'用户类型  1 普通用户 2管理员 4超级管理员' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'User', @level2type=N'COLUMN',@level2name=N'UserType'
GO
USE [master]
GO
ALTER DATABASE [OrderService] SET  READ_WRITE 
GO
