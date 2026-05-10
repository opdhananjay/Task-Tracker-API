    CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),

    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(50) NULL,

    Email NVARCHAR(150) NOT NULL UNIQUE,

    PasswordSalt NVARCHAR(500) NOT NULL,
    PasswordHash NVARCHAR(500) NOT NULL,

    Role NVARCHAR(50) NOT NULL,

    OrganizationId INT NULL,

    Department NVARCHAR(100) NULL,
    PhoneNumber NVARCHAR(20) NULL,
    EmployeeId NVARCHAR(50) NULL,

    CompanyName NVARCHAR(150) NULL,

    IsActive BIT NOT NULL DEFAULT 1,

    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL
    
)



CREATE TABLE Tasks(
	Id INT PRIMARY KEY IDENTITY(1,1),
	Title NVARCHAR(100) NOT NULL,
	Description NVARCHAR(1000) NOT NULL,
	TaskType NVARCHAR(50) NOT NULL,
	DeveloperId INT NULL,
	TesterId INT NULL,
	CreatedBy INT NOT NULL,
	StartDateTime DATETIME NULL,
    DueDateTime DATETIME NULL,
	Priority NVARCHAR(50) NOT NULL,
	Status NVARCHAR(50) NOT NULL,
	UnitTestingStatus NVARCHAR(50) NULL,
    AcceptanceCriteria NVARCHAR(1000) NULL,
	CreatedAt DATETIME DEFAULT GETDATE(),
	UpdatedAt DATETIME NULL,
	FOREIGN KEY (DeveloperId) References Users(Id),
	FOREIGN KEY (TesterId) References Users(Id),
	FOREIGN KEY (CreatedBy) REFERENCES Users(Id)
)


CREATE TABLE Organizations(
    Id INT PRIMARY KEY IDENTITY(1,1),
    OrganizationName NVARCHAR(150) NOT NULL,
    Email NVARCHAR(150) NULL,
    PhoneNumber NVARCHAR(20) NULL,
    Address NVARCHAR(500) NULL,
    OrganizationType NVARCHAR(100) NULL,
    CreatedBy INT NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,

    FOREIGN KEY (CreatedBy) REFERENCES Users(Id)
)


ALTER TABLE Users
ADD OrganizationId INT NULL


ALTER TABLE Users
ADD CONSTRAINT FK_Users_Organizations
FOREIGN KEY (OrganizationId)
References Organizations(Id)




