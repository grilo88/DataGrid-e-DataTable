USE [Teste]
GO

/****** Object:  Table [dbo].[Tabela]    Script Date: 14/06/2019 17:19:47 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Tabela](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Nome] [varchar](50) NULL,
	[Sobrenome] [varchar](50) NULL,
	[Idade] [int] NULL,
	[Sexo] [bit] NULL,
 CONSTRAINT [PK_Tabela] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

