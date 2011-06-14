#!/usr/bin/php
<?php
	$link=pg_connect("host=db port=5432 user=g1027131_u password=YRAkDJ2Xcr dbname=g1027131_u");
	$username = $_POST["username"];
	$password = $_POST["password"];
	if (!$link)
	{
		exit("<error>Database connection failed</error>");
	}
	$query = "SELECT playernumber, playing FROM Players WHERE Players.username = '" . $username . "' AND Players.password = '" . $password . "'";
	$result = pg_query($link, $query);
	$rows = pg_num_rows($result);
	if ($rows > 0)
	{
		$playing = pg_fetch_result($result, 0, 'playing');
		$playerno = pg_fetch_result($result, 0, 'playernumber');
		if (!$playing)
		{
			pg_close($link);
			exit("<error>This login is already playing</error>"); 
		}
		$query = "UPDATE Players SET playing = 'true' WHERE username = '" . $username . "' AND Players.password = '" . $password . "'";
		$result = pg_query($link, $query);
		if (!$result)
		{
			pg_close($link);
			exit("<error>Updating players state failed</error>");
		}
		setcookie("playernumber", $playerno, 0, "/project/2010/271/g1027131/", "www.doc.ic.ac.uk");
	}
	else
	{
		pg_close($link);
		exit("<error>Username and password are incorrect</error>");
	}
	pg_close($link);
	echo ("<success></success>");
?>
