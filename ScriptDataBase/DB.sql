USE [master]
GO
/****** Object:  Database [TestePratico]    Script Date: 04/11/2024 16:34:08 ******/
CREATE DATABASE [TestePratico]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'TestePratico', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\TestePratico.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'TestePratico_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\TestePratico_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [TestePratico] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [TestePratico].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [TestePratico] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [TestePratico] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [TestePratico] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [TestePratico] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [TestePratico] SET ARITHABORT OFF 
GO
ALTER DATABASE [TestePratico] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [TestePratico] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [TestePratico] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [TestePratico] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [TestePratico] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [TestePratico] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [TestePratico] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [TestePratico] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [TestePratico] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [TestePratico] SET  DISABLE_BROKER 
GO
ALTER DATABASE [TestePratico] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [TestePratico] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [TestePratico] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [TestePratico] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [TestePratico] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [TestePratico] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [TestePratico] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [TestePratico] SET RECOVERY FULL 
GO
ALTER DATABASE [TestePratico] SET  MULTI_USER 
GO
ALTER DATABASE [TestePratico] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [TestePratico] SET DB_CHAINING OFF 
GO
ALTER DATABASE [TestePratico] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [TestePratico] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [TestePratico] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [TestePratico] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'TestePratico', N'ON'
GO
ALTER DATABASE [TestePratico] SET QUERY_STORE = OFF
GO
USE [TestePratico]
GO
/****** Object:  Table [dbo].[Usuario]    Script Date: 04/11/2024 16:34:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Usuario](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Email] [varchar](200) NOT NULL,
	[Senha] [varchar](300) NOT NULL,
	[ChaveSenha] [varchar](300) NOT NULL,
 CONSTRAINT [PK_Usuario] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[spUsuarioInsert]    Script Date: 04/11/2024 16:34:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[spUsuarioInsert]
(
	  @Email					varchar(200)	= null
	, @Senha					varchar(300)	= null
	, @ChaveSenha				varchar(300)	= null
)
as
begin
	declare @ID bigint = null
	insert into Usuario with(rowlock)
	(
		  Email
		, Senha
		, ChaveSenha
	)	
	values
	(
		  @Email
		, @Senha
		, @ChaveSenha
	)
	select @@IDENTITY as Id
end
GO
/****** Object:  StoredProcedure [dbo].[spUsuarioSelect]    Script Date: 04/11/2024 16:34:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[spUsuarioSelect]
(
	  @Id			bigint			= null
	, @Email		varchar(200)	= null
)
as
begin
	set nocount on
	select Id
		 , Email
		 , Senha
		 , ChaveSenha
	from 
		Usuario with(nolock) 
	where
		(@Id is null or Id=@Id)
	and
		(@Email is null or Email = @Email)
	order by
		  Email
	asc
end
GO
USE [master]
GO
ALTER DATABASE [TestePratico] SET  READ_WRITE 
GO
