using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json.Serialization;

namespace Travelling
{
    public class Traveler : ICloneable
    {
        private string _name;
        private string _currentLocation;
        private List<string> _route;

        public string name
        {
            get { return _name; } 
            set { _name = value; } 
        }
        public string currentLocation
        {
            get { return _currentLocation; }
            set { _currentLocation = FormatName(value); }
        }
        public List<string> route
        {
            get { return _route; } 
            set { _route = value; }
        }

        public Traveler(string name)
        {
            _name = name;
            _currentLocation = string.Empty;
            _route = new List<string>();
        }

        public string this[int index] => _route[index];

        public static bool operator ==(Traveler a, Traveler b)
        {
            if (ReferenceEquals(a, b))
                return true;

            if (a is null || b is null)
                return false;

            return a._name == b._name && a._currentLocation == b._currentLocation;
        }
        public static bool operator !=(Traveler a, Traveler b) => !(a == b);
        private static string FormatName(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
                return string.Empty;

            TextInfo textInfo = CultureInfo.InvariantCulture.TextInfo;
            return textInfo.ToTitleCase(city.ToLowerInvariant());
        }
        public object Clone()
        {
            return new Traveler(_name)
            {
                _currentLocation = _currentLocation,
                _route = new List<string>(_route)
            };
        }

        public void SaveToFile(string filePath)
        {
            string routeJson = JsonSerializer.Serialize(this.route);
            string data = "{\n" +
                "  \"name\": \"" + this.name + "\",\n" +
                "  \"currentLocation\": " + 
                    (string.IsNullOrEmpty(this.currentLocation) ? "null" : JsonSerializer.Serialize(this.currentLocation)) + ",\n" +
                "  \"route\": " + routeJson + "\n" +
                "}";
            File.WriteAllText(filePath, data);
        }

        public static Traveler LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("File not found");
            }
            string jsonString = File.ReadAllText(filePath);

            var options = new JsonSerializerOptions
            {
                UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow 
            };

            Traveler traveler;

            try
            {
                traveler = JsonSerializer.Deserialize<Traveler>(jsonString, options);
            }
            catch (JsonException)
            {
                throw new FileLoadException("Invalid travel data");
            }

            if (traveler == null)
                throw new FileLoadException("Traveler data is empty or could not be read.");

            if (string.IsNullOrWhiteSpace(traveler.name))
                throw new FileLoadException("Missing required field: 'name'.");

            traveler.currentLocation ??= string.Empty;

            traveler.route ??= new List<string>();

            return traveler;
        }

        public string GetName() => _name;
        public void SetLocation(string newLocation) => _currentLocation = FormatName(newLocation);
        public string GetLocation() => _currentLocation;
        public void AddCity(string city)
        {
            string formattedCity = FormatName(city);
            if (string.IsNullOrEmpty(formattedCity))
                throw new Exception("City name is invalid.");

            _route.Add(formattedCity);
        }
        public bool RemoveCity(string city)
        {
            string formattedCity = FormatName(city);
            if (string.IsNullOrEmpty(formattedCity))
                return false;

            return _route.Remove(formattedCity);
        }
        public bool HasCity(string city)
        {
            string formattedCity = FormatName(city);
            if (string.IsNullOrEmpty(formattedCity))
                return false;

            return _route.Contains(formattedCity);
        }
        public string GetRoute() => string.Join(" -> ", _route);
        public void ClearRoute() => _route.Clear();    
        public void SortRoute() => _route.Sort();

        public bool PlanRouteTo(string destination, CityGraph map)
        {
            string startPoint = null;
            if (currentLocation != null)
            {
                startPoint = currentLocation;
            }
            else if (route.Count > 0)
            {
                startPoint = route[0];
            }

            if (startPoint == null)
            {
                Console.WriteLine("No route!");
                return false;
            }

            List<string> newPath = map.FindShortestPath(startPoint, destination);
            
            if (newPath != null)
            {
                route = newPath;
                return true;
            }
            return false;
        }
        public int GetStopCount() => _route.Count;
        public string GetNextStop()
        {
            if (route.Count > 0)
            {
                return route[0];
            }
            return null;
        }
        public override string ToString()
        {
            return $"Traveler: {_name} | Location: {_currentLocation} | Route: {GetRoute()}";
        }

        public string GetLastStop()
        {
            if (route.Count > 0)
            {
                return route[route.Count - 1];
            }
            return null;
        }
    }
}
