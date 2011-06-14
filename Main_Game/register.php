#!/usr/bin/php
<?php
	if($_POST['name'] && $_POST['password']) 
    { 
		$name = $_POST['name'];
		$password = $_POST['password'];
    }
	else
	{
		exit("<error>Both a username and password must be added</error>");
	}
	$db = pg_connect('host=db port=5432 dbname=g1027131_u user=g1027131_u password=YRAkDJ2Xcr'); 
	if(!$db) 
	{
		exit("<error>Connection to database failed</error>");
	}
	$query = "SELECT * FROM Players WHERE username='$name'";
	$result = pg_query($db, $query); 
    if (!$result) 
	{
		pg_close($db);
		exit("<error>Query failed unexpectedly</error>"); 
    }
	$rows = pg_num_rows($result);
	if ($rows == 0)
	{
		$query = "INSERT INTO Players (username, password) VALUES ('" . $name . "', '" . $password . "');";
		$result = pg_query($db, $query);
		if (!$result)
		{
			pg_close($db);
			exit("<error>Query failed unexpectedly</error>"); 
		}
		pg_close($db);
		exit ("<success></success>");
	}
	else
	{
		pg_close($db);
		exit ("<validity>Username is already in use - choose another</validity>");
	}
?>
