/****** 
This script will recreate the MusicStore DB CartItems table with the purpose of adding an index on the CartId column
This is necesary because MusicStore queries on that column and performance is severely compromised  when the 
table gets large.   The changes are:
-  Change the CartId type from nvarchar to uniqueidentifier
-  Add a non clustered index on CartId

This change should be made for versions of MusicStore that have had their corresponding CartId object type changed
in their datamodel as well. 
******/

ALTER TABLE [dbo].[CartItems] DROP CONSTRAINT [FK_CartItems_Albums_AlbumId]
GO

/****** Object:  Table [dbo].[CartItems]    Script Date: 6/28/2016 3:38:45 PM ******/
DROP TABLE [dbo].[CartItems]
GO

/****** Object:  Table [dbo].[CartItems]    Script Date: 6/28/2016 3:38:45 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CartItems](
	[CartItemId] [int] IDENTITY(1,1) NOT NULL,
	[AlbumId] [int] NOT NULL,
	[CartId] [uniqueidentifier] NOT NULL,
	[Count] [int] NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_CartItems] PRIMARY KEY CLUSTERED 
(
	[CartItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO

ALTER TABLE [dbo].[CartItems]  WITH CHECK ADD  CONSTRAINT [FK_CartItems_Albums_AlbumId] FOREIGN KEY([AlbumId])
REFERENCES [dbo].[Albums] ([AlbumId])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[CartItems] CHECK CONSTRAINT [FK_CartItems_Albums_AlbumId]
GO

CREATE INDEX IX_CartItems_CartId
ON [CartItems] 
(
	[CartId]
)
GO

