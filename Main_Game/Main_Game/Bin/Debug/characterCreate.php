#!/usr/bin/php
<?php
	$link=pg_connect("host=db port=5432 user=g1027131_u password=YRAkDJ2Xcr dbname=g1027131_u");
	if (!$link)
	{
		pg_close($link);
		exit("<error>Error: Database connection failed</error>");
	}
	$query = "SELECT name FROM Characters WHERE name = '" . $_POST["name"] . "';";
	$result = pg_query($link, $query);
	$rows = pg_num_rows($result);
	if ($rows > 0)
	{
		pg_close($link);
		exit("<error>This character name is taken please choose another name</error>");
	}
	$playerno = $_COOKIE["playernumber"];
	$query = "INSERT INTO Characters (name, classid, lvl, exptonext, maxhealth, currenthealth, maxmana, currentmana, strength, agility, intelligence, 		playernumber) VALUES ('" . $_POST["name"] . "', '" . $_POST["classid"] . "', '1', '" . $_POST["exptonext"] . "', '" . 
	$_POST["maxhealth"] . "', '" . $_POST["maxhealth"] . "', '" . $_POST["maxmana"] . "', '" . $_POST["maxmana"] . "', '" . $_POST["strength"] . "', '" 
	. $_POST["agility"] . "', '" . $_POST["intelligence"] . "', '" . $playerno . "') RETURNING characterid;";
	$result = pg_query($link, $query);
	if (!$result)
	{
		pg_close($link);
		exit("<error>Error: Query failed</error>");
	}
	$characterid = pg_fetch_result($result, 0, 0);
	$n = 0;
	while (isset($_POST["ability" . $n]))
	{
		$query = "INSERT INTO CharacterAbility (characterid, name) VALUES ('" . $characterid . "', '" . $_POST["ability" . $n] . "');";
		$result = pg_query($link, $query);
		$n++;
	}
	pg_close($link);
	exit("<success></success>");
?>