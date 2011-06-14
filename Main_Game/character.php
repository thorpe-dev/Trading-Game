#!/usr/bin/php
<?php
	$link=pg_connect("host=db port=5432 user=g1027131_u password=YRAkDJ2Xcr dbname=g1027131_u");
	if (!$link)
	{
		pg_close($link);
		exit("<error>Database connection failed</error>");
	}
	$playerno = $_COOKIE["playernumber"];
	$query = "SELECT * FROM Characters WHERE Characters.playernumber = '" . $playerno . "'";
	$result = pg_query($link, $query);
	$rows = pg_num_rows($result);
	if ($rows > 0)
	{
		$values = pg_fetch_array($result, 0, PGSQL_ASSOC);
		$characterdetails = "<success><character><characterid>" . $values["characterid"] . "</characterid><name>" . $values["name"] . "</name><classid>"
				. $values["classid"] . "</classid><lvl>" . $values["lvl"] . "</lvl><exptonext>" . $values["exptonext"] . "</exptonext><maxhealth>"
				. $values["maxhealth"] . "</maxhealth><currenthealth>" . $values["currenthealth"] . "</currenthealth><maxmana>" 
				. $values["maxmana"] . "</maxmana><currentmana>" . $values["currentmana"] . "</currentmana><strength>"
				. $values["strength"] . "</strength><agility>" . $values["agility"] . "</agility><intelligence>" . $values["intelligence"] .
				"</intelligence>";
		$query = "SELECT name FROM CharacterAbility WHERE characterid = '" . $values["characterid"] . "';";
		$result = pg_query($link, $query);
		$noOfAbilities = pg_num_rows($result);
		$abilitydetails = "<abilities>";
		for ($n = 0; $n < $noOfAbilities; $n++)
		{
			$values = pg_fetch_array($result, $n, PGSQL_ASSOC);
			$abilitydetails = $abilitydetails . "<ability>" . $values["name"] . "</ability>";
			
		}
		$abilitydetails = $abilitydetails . "</abilities>";
		$terminator = "</character></success>";
		$xml = $characterdetails . $abilitydetails . $terminator;
	}
	else
	{
		pg_close($link);
		exit("<success></success>");
	}
	pg_close($link);
	echo $xml;
?>