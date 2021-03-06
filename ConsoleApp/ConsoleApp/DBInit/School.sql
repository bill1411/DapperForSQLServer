---------创建库------------
Create database School
go

USE [School]
GO
----------------插入用户信息----------------
CREATE PROCEDURE [dbo].[sp_Add_Student_Source]  
    @Id int output,
    @name nvarchar(10),     --姓名
	@subject nvarchar(10),  --学科
	@source int             --得分
AS
BEGIN
	insert into test(name,subject,source) values(@name,@subject,@source)
	SET @Id = cast(scope_identity() as int) 
END

GO
/****** Object:  StoredProcedure [dbo].[sp_Get_Student_Info]    Script Date: 2020-03-12 10:57:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

----------------获取客户信息----------------
CREATE PROCEDURE [dbo].[sp_Get_Student_Info]  
    @name nvarchar(10) 
AS
BEGIN
	select * from userInfo where StuName = @name ;   
	select * from test where name = @name; 
END

GO
/****** Object:  Table [dbo].[test]    Script Date: 2020-03-12 10:57:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[test](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](50) NULL,
	[subject] [varchar](50) NULL,
	[source] [decimal](18, 2) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UserInfo]    Script Date: 2020-03-12 10:57:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[UserInfo](
	[StuNo] [int] IDENTITY(1000,1) NOT NULL,
	[StuName] [varchar](8) NOT NULL,
	[StuSex] [int] NOT NULL,
	[StuAddress] [varchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[StuNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO


-------------初始化数据---------------
insert into userInfo values('张三',1,'凤城八路')
insert into userInfo values('李四',1,'凤城九路')
insert into userInfo values('王五',1,'凤城十路')
insert into userInfo values('刘六',1,'凤城七路')

insert into test values('张三','语文',86)
insert into test values('张三','数学',85)
insert into test values('李四','语文',84)
insert into test values('李四','数学',83)
insert into test values('王五','语文',82)
insert into test values('王五','数学',81)
