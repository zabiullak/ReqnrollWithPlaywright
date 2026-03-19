Feature: Weather-New
	Check the Weather of Bangalore

@LocalWeather
@Test-004
Scenario: Check My Local Weather - 3
	Given i navigate to "https://www.bbc.com/weather"
	When i input the location "Bangalore, India"
	And  click search
	Then i see current weather for "Bangalore"

@LocalWeather
@Test-005
Scenario: Check My Local Weather - 4
	Given i navigate to "https://www.bbc.com/weather"
	When i input the location "Chennai International Airport, India"
	And  click search
	Then i see current weather for "Chennai"

@Test-006
Scenario: Check News page -2
	Given i navigate to "https://www.bbc.com/weather"
	When i click on News link
	Then i see the News page