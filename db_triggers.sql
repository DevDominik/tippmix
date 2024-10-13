DELIMITER //

CREATE TRIGGER AddDefaultPerms
AFTER INSERT ON Bettors
FOR EACH ROW
BEGIN
    DECLARE perm_id INT;
    DECLARE done INT DEFAULT 0;
    DECLARE cur CURSOR FOR 
    SELECT PermID FROM PermSettings;
    DECLARE CONTINUE HANDLER FOR NOT FOUND SET done = 1;
    OPEN cur;
    read_loop: LOOP
        FETCH cur INTO perm_id;
        IF done THEN
            LEAVE read_loop;
        END IF;
        INSERT INTO Perms (BettorID, PermID)
        VALUES (NEW.BettorsID, perm_id);
    END LOOP;
    CLOSE cur;
END;

//
DELIMITER ;

DELIMITER //

CREATE TRIGGER AddPermToAllBettors
AFTER INSERT ON PermSettings
FOR EACH ROW
BEGIN
    DECLARE bettor_id INT;
    DECLARE done INT DEFAULT 0;
    DECLARE cur CURSOR FOR 
    SELECT BettorsID FROM Bettors;
    DECLARE CONTINUE HANDLER FOR NOT FOUND SET done = 1;
    OPEN cur;
    read_loop: LOOP
        FETCH cur INTO bettor_id;
        IF done THEN
            LEAVE read_loop;
        END IF;
        INSERT INTO Perms (BettorID, PermID)
        VALUES (bettor_id, NEW.PermID);
    END LOOP;
    CLOSE cur;
END;

//
DELIMITER ;
