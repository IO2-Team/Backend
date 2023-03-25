-- Created by Vertabelo (http://vertabelo.com)
-- Last modification date: 2023-03-07 19:02:54.689

-- tables
-- Table: Categories
CREATE TABLE Categories (
    id Serial  NOT NULL,
    name varchar(250)  NOT NULL,
    CONSTRAINT Categories_pk PRIMARY KEY (id)
);

-- Table: EmailCodes
CREATE TABLE EmailCodes (
    id Serial  NOT NULL,
    organizer_id int  NOT NULL,
    code varchar(255)  NOT NULL,
    time timestamp  NOT NULL,
    CONSTRAINT EmailCodes_pk PRIMARY KEY (id)
);

-- Table: EventInCategories
CREATE TABLE EventInCategories (
    id Serial  NOT NULL,
    event_id int  NOT NULL,
    categories_id int  NOT NULL,
    CONSTRAINT EventInCategories_pk PRIMARY KEY (id)
);

-- Table: Events
CREATE TABLE Events (
    id Serial  NOT NULL,
    owner int  NOT NULL,
    title varchar(250)  NOT NULL,
    name varchar(250)  NULL,
    startTime timestamp  NOT NULL,
    endTime timestamp  NOT NULL,
    latitude varchar(20)  NOT NULL,
    longitude varchar(20)  NOT NULL,
    status int  NOT NULL,
    placeCapacity int  NOT NULL,
    placeSchema text  NULL,
    CONSTRAINT id PRIMARY KEY (id)
);

-- Table: Organizers
CREATE TABLE Organizers (
    id Serial  NOT NULL,
    name varchar(320)  NOT NULL,
    email varchar(320)  NOT NULL,
    password text  NOT NULL,
    status int  NOT NULL,
    CONSTRAINT Organizers_pk PRIMARY KEY (id)
);

-- Table: Reservatons
CREATE TABLE Reservatons (
    event_id int  NOT NULL,
    place_id int  NOT NULL,
    token int  NOT NULL,
    CONSTRAINT Reservatons_pk PRIMARY KEY (event_id,place_id)
);

-- Table: Sessions
CREATE TABLE Sessions (
    id Serial  NOT NULL,
    organizer_id int  NOT NULL,
    token varchar(255)  NOT NULL,
    time timestamp  NOT NULL,
    CONSTRAINT Sessions_pk PRIMARY KEY (id)
);

-- foreign keys
-- Reference: EmailCodes_Organizers (table: EmailCodes)
ALTER TABLE EmailCodes ADD CONSTRAINT EmailCodes_Organizers
    FOREIGN KEY (organizer_id)
    REFERENCES Organizers (id)  
    NOT DEFERRABLE 
    INITIALLY IMMEDIATE
;

-- Reference: Events_Organizers (table: Events)
ALTER TABLE Events ADD CONSTRAINT Events_Organizers
    FOREIGN KEY (owner)
    REFERENCES Organizers (id)  
    NOT DEFERRABLE 
    INITIALLY IMMEDIATE
;

-- Reference: Reservatons_Events (table: Reservatons)
ALTER TABLE Reservatons ADD CONSTRAINT Reservatons_Events
    FOREIGN KEY (event_id)
    REFERENCES Events (id)  
    NOT DEFERRABLE 
    INITIALLY IMMEDIATE
;

-- Reference: Sessions_Organizers (table: Sessions)
ALTER TABLE Sessions ADD CONSTRAINT Sessions_Organizers
    FOREIGN KEY (organizer_id)
    REFERENCES Organizers (id)  
    NOT DEFERRABLE 
    INITIALLY IMMEDIATE
;

-- Reference: eventInCategories_Categories (table: EventInCategories)
ALTER TABLE EventInCategories ADD CONSTRAINT eventInCategories_Categories
    FOREIGN KEY (categories_id)
    REFERENCES Categories (id)  
    NOT DEFERRABLE 
    INITIALLY IMMEDIATE
;

-- Reference: eventInCategories_Event (table: EventInCategories)
ALTER TABLE EventInCategories ADD CONSTRAINT eventInCategories_Event
    FOREIGN KEY (event_id)
    REFERENCES Events (id)  
    NOT DEFERRABLE 
    INITIALLY IMMEDIATE
;

-- End of file.

