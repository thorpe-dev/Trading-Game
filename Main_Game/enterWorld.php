#!/usr/bin/php
<?php
	$link=pg_connect("host=db port=5432 user=g1027131_u password=YRAkDJ2Xcr dbname=g1027131_u");
	if (!$link)
	{
		exit("<error>Database connection failed</error>");
	}
	$playerid = $_COOKIE["playernumber"];
	$query = "SELECT characterid FROM Characters WHERE playernumber = '" . $playerid . "';";
	$result = pg_query($link, $query);
	$characterid = pg_fetch_result($result, 0, 'characterid');
	setcookie("characterid", $characterid, 0, "/project/2010/271/g1027131/", "www.doc.ic.ac.uk");
	$query = "SELECT characterid FROM Location WHERE characterid = '" . $characterid . "';";
	$result = pg_query($link, $query);
	$query = "DELETE FROM Location WHERE characterid = '" . $characterid . "';" .
			 "INSERT INTO Location (Characterid, placeid) VALUES ('" . $characterid . "', '0');";
	$result = pg_query($link, $query);
	if (!$result)
	{
		pg_close($link);
		exit("<error>Could not create location</error>");
	}
	else
	{
		pg_close($link);
		echo "<success></success>";
	}
?>