#!/usr/bin/php

<?php
	$link=pg_connect("host=db port=5432 user=g1027131_u password=YRAkDJ2Xcr dbname=g1027131_u");
	if (!$link)
	{
		exit("<error>Database connection failed</error>");
	}
	$playerno = $_COOKIE["playernumber"];
	$query = "SELECT * FROM Characters WHERE Characters.playernumber = '" . $playerno . "'";
	$result = pg_query($link, $query);
	$rows = pg_num_rows($result);
	if ($rows > 0)
	{
		$values = pg_fetch_array($result, 0, PGSQL_ASSOC);
		$xml = "<success><character><characterid>" . $values["characterid"] . "</characterid><name>" . $values["name"] . "</name><classid>"
				. $values["classid"] . "</classid><lvl>" . $values["lvl"] . "</lvl><exptonext>" . $values["exptonext"] . "</exptonext><strength>"
				. $values["strength"] . "</strength><agility>" . $values["agility"] . "</agility><intelligence>" . $values["intelligence"] .
				"</intelligence></character></success>";
		
	}
	else
	{
		exit("<success></success>");
	}
	pg_close($link);
	echo $xml;
?>