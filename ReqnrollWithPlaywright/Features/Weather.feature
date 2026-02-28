Feature: Weather
	Check the Weather of Bangalore

@LocalWeather
@Test-001
Scenario: Check My Local Weather - 1
	Given i navigate to "https://www.bbc.com/weather"
	When i input the location "Bangalore, India"
	And  click search
	Then i see current weather for "Bangalore"

@LocalWeather
@Test-002
Scenario: Check My Local Weather - 2
	Given i navigate to "https://www.bbc.com/weather"
	When i input the location "Bangalore, India"
	And  click search
	Then i see current weather for "Bangalore"