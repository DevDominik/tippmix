CREATE TABLE Bettors (
	BettorsID INT AUTO_INCREMENT PRIMARY KEY,  
	Username VARCHAR(50) NOT NULL,
	Password VARCHAR(255),              
	Balance INT NOT NULL,                      
	Email VARCHAR(100) NOT NULL,               
	JoinDate DATE NOT NULL,                     
	IsActive BOOLEAN NOT NULL DEFAULT 1        
);

CREATE TABLE Events (
	EventID INT AUTO_INCREMENT PRIMARY KEY,     
	EventName VARCHAR(100) NOT NULL,            
	EventDate DATE NOT NULL,                    
	Category VARCHAR(50) NOT NULL,             
	Location VARCHAR(100) NOT NULL             
);

CREATE TABLE Bets (
	BetsID INT AUTO_INCREMENT PRIMARY KEY,   
	BetDate DATE NOT NULL,                   
	Odds FLOAT NOT NULL,                     
	Amount INT NOT NULL,                     
	BettorsID INT NOT NULL,                  
	EventID INT NOT NULL,                    
	Status BOOLEAN NOT NULL,             
	FOREIGN KEY (BettorsID) REFERENCES Bettors(BettorsID),
	FOREIGN KEY (EventID) REFERENCES Events(EventID)
);

CREATE TABLE PermSettings (
	PermID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,     
	DisplayName VARCHAR(100) NOT NULL,            
	PermissibilityLevel INT NOT NULL,
	RoleIconName VARCHAR(50) NOT NULL
);

CREATE TABLE Perms (
	ID BIGINT(32) NOT NULL PRIMARY KEY AUTO_INCREMENT, 
	BettorID INT NOT NULL, 
	PermID INT NOT NULL,
	IsActive BOOLEAN NOT NULL,
	FOREIGN KEY (BettorID) REFERENCES bettors(BettorsID),
	FOREIGN KEY (PermID) REFERENCES permsettings(PermID)
);

CREATE TABLE Organizations (
	OrgID INT NOT NULL PRIMARY KEY,
	DisplayName VARCHAR(255) NOT NULL,
	Balance INT NOT NULL
);

CREATE TABLE OrgAssociates (
	AssociateID INT NOT NULL PRIMARY KEY,
	BelongsToOrgID INT NOT NULL,
	FOREIGN KEY (BelongsToOrgID) REFERENCES Organizations(OrgID)
);

CREATE TABLE RoleKey (
	KeyID CHARACTER(12) NOT NULL PRIMARY KEY,
	ForRole INT NOT NULL,
	IsActive BOOLEAN NOT NULL,
	FOREIGN KEY (ForRole) REFERENCES permsettings(PermID)
);

CREATE TABLE EventToOrgLink (
	EventID INT NOT NULL PRIMARY KEY,
	OrgID INT NOT NULL,
	FOREIGN KEY (OrgID) REFERENCES organizations(OrgID)
);
