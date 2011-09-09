SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Multiply]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
BEGIN
execute dbo.sp_executesql @statement = N'  create function [dbo].[Multiply](@n1 int, @n2 int) returns int as  begin  	declare @num3 int;  	set @num3 = @n1*@n2;  	return @num3;  end;  ' 
END

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CalcLength_P]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[CalcLength_P]
@name VARCHAR(255)
, @strlength INT OUTPUT
AS
BEGIN
	SET @strlength = DataLength(@name);
END;
' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ReturnUserTable_F]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
BEGIN
execute dbo.sp_executesql @statement = N'-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[ReturnUserTable_F]
(
	@howmuch int
)
RETURNS 
@userTable TABLE 
(
	-- Add the column definitions for the TABLE variable here
	[user] varchar(50), 
	[username] varchar(255)
)
AS
BEGIN
	-- Fill the table variable with the rows for your result set
	DECLARE @i INT
	SET @i = 0
	WHILE (@i < @howmuch)
	BEGIN
		SET @i = @i + 1
		INSERT @userTable([user], [username])
			VALUES(''User '' + CAST(@i AS VARCHAR(10)), ''Username '' + CAST(@i AS VARCHAR(10)))
	END
	
	RETURN 
END
' 
END

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ConcatenateStrings_P]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[ConcatenateStrings_P]
@firstString varchar(255)
,@secondString varchar(255)
,@concatenated varchar(600) output
AS
BEGIN
	SET @concatenated = @firstString + '' '' + @secondString
END
' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ConcatenateStrings_F]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
BEGIN
execute dbo.sp_executesql @statement = N'
CREATE FUNCTION [dbo].[ConcatenateStrings_F]
(
@firstString varchar(255)
,@secondString varchar(255)
)
RETURNS VARCHAR(600)
AS
BEGIN
	DECLARE @concatenated VARCHAR(600)
	SET @concatenated = @firstString + '' '' + @secondString
	RETURN @concatenated
END
' 
END

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Users](
	[name] [varchar](50) NULL,
	[username] [varchar](50) NULL,
	[userid] [int] IDENTITY(1,1) NOT NULL
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PopulateUserTable_P]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[PopulateUserTable_P]
@howmuch INT
AS
BEGIN
	-- Fill the table variable with the rows for your result set
	DECLARE @i INT
	SET @i = 0
	WHILE (@i < @howmuch)
	BEGIN
		SET @i = @i + 1
		INSERT [Users]([name], [username])
			VALUES(''User '' + CAST(@i AS VARCHAR(10)), ''Username '' + CAST(@i AS VARCHAR(10)))
	END
END
' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OpenCrsr_P]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[OpenCrsr_P]
@howmuch INT,
@OutCrsr CURSOR VARYING OUTPUT
AS
BEGIN
	SET @OutCrsr = CURSOR FOR
	SELECT TOP (@howmuch) [name], [username], [userid]
	FROM [Users];

	OPEN @OutCrsr;
END
' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteUserTable_P]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[DeleteUserTable_P]
AS
DELETE [Users];
' 
END
GO

CREATE PROCEDURE [dbo].[TestProc2]
	@iddocument int,
	@iddestination_user int
as
declare @errorsave int

set @errorsave = 0

if (@iddocument < 100)
begin
	set @errorsave = 53120
	raiserror(@errorsave, 15, 1, 'Custom error message')
	return @errorsave
end
go

exec sp_addmessage @msgnum = 53120, @severity=1, @msgtext = 'test user defined error msg' 

GO
CREATE procedure [dbo].[ListUsers_P] @howmuch int AS
BEGIN
select top (@howmuch) * from users order by userid
END;
GO

create procedure MultiplyIO(@factor int, @val int output) as
begin
	set @val = @factor*@val;
end;

GO

create procedure TestDecimal
@inParam decimal(15, 8),
@copyOfInParam decimal(15, 8) out,
@constOutParam decimal(15, 8) out
as
begin
set @copyOfInParam = @inParam
set @constOutParam = 123.456;
end;

GO

Create procedure TestReturn
@inParam int
as
begin
	return @inParam
end
GO

alter PROC up_vb
	@vb varbinary(16)
as
	select cast(0x1020304050 as varbinary(16)) as d2
GO