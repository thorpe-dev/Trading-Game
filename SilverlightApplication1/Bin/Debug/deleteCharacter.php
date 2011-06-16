#!/usr/bin/php
<?php
	$link=pg_connect("host=db port=5432 user=g1027131_u password=YRAkDJ2Xcr dbname=g1027131_u");
	if (!$link)
	{
		exit("<error>Database connection failed</error>");
	}
	$playerno = $_COOKIE["playernumber"];
	$query = "DELETE FROM Characters WHERE playernumber = '" . $playerno . "';";
	$result = pg_query($link, $query);
	if ($result)
	{
		pg_close($link);
		exit("<success></success>");
	}
	else
	{
		pg_close($link);
		exit("<error>Query failed</error>");
	}
?>