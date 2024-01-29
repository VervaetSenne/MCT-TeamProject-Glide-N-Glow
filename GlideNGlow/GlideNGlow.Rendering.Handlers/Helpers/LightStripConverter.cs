using GlideNGlow.Common.Models;

namespace GlideNGlow.Rendering.Handlers.Helpers;

public class LightStripConverter
{
    private List<LightstripData> _lightStrips = new();
    private readonly List<float> _lightStripDistanceStarts = new();
    private readonly List<int> _lightStripIdStarts = new();
    private readonly List<float> _lightStripDistanceEnds = new();
    private readonly List<int> _lightStripIdEnds = new();
    private readonly List<float> _lightStripPixelDensity = new();
    
    public LightStripConverter(List<LightstripData> lightstrips)
    {
        UpdateLightStripData(lightstrips);
    }
    
    public void UpdateLightStripData(List<LightstripData> lightstrips)
    {
        _lightStrips = lightstrips;
        
        _lightStripDistanceStarts.Clear();
        _lightStripDistanceEnds.Clear();
        
        float currentStart = 0;
        var currentId = 0;
        foreach (var lightStrip in _lightStrips)
        {
            currentStart += lightStrip.DistanceFromLast;
            _lightStripDistanceStarts.Add(currentStart);
            _lightStripIdStarts.Add(currentId);
            
            
            currentStart += lightStrip.Length;
            currentId += lightStrip.Leds;
            _lightStripDistanceEnds.Add(currentStart);
            _lightStripIdEnds.Add(currentId);
            
            _lightStripPixelDensity.Add(lightStrip.Length / lightStrip.Leds);
        }
    }
    
    //function tries to find an id (int) and puts it in the out parameter
    public bool TryConvertToPixelPosition(float distance, out int id)
    {
        //check if distance is smaller than 0, if so modulo the distance by the last _lightStripDistanceEnds
        distance = FitWithinRange(distance);
        
        //find the first _lightStripDistanceEnds that is greater than distance
        var index = _lightStripDistanceEnds.FindIndex(x => x > distance);
        if (index == -1)
        {
            id = -1;
            return false;
        }
        //check if the _lightStripDistanceStarts[index] is also larger than distance, if so it's not on a lightstrip
        if (_lightStripDistanceStarts[index] > distance)
        {
            id = -1;
            return false;
        }
        
        //if we get here we know that the distance is on a lightstrip, now we must find the exact location
        
        id = FindExactLocation(distance, index);
        return true;
    }

    private int FindExactLocation(float distance, int index)
    {
        //get the distance from the start of the lightstrip
        var distanceFromStart = distance - _lightStripDistanceStarts[index];
        //get the id from the start of the lightstrip
        var idFromStart = _lightStripIdStarts[index];
        //get the pixel density of the lightstrip
        var pixelDensity = _lightStripPixelDensity[index];
        //get the id of the pixel
        var pixelId = (int)(distanceFromStart / pixelDensity);
        //add the pixel id to the id from the start of the lightstrip
        return idFromStart + pixelId;
    }

    public bool TryConvertToPixelLine(float start, float end, out int idStart, out int idStop)
    {
        start = FitWithinRange(start);
        
        end = FitWithinRange(end);
        var displayable = false;
        
        //find the first _lightStripDistanceEnds that is greater than start
        var indexStart = _lightStripDistanceEnds.FindIndex(x => x > start);
        if (indexStart == -1)
        {
            //something went quite wrong, normally we shouldn't ever get here.
            idStart = -1;
            idStop = -1;
            return false;
        }
        
        //check if the _lightStripDistanceStarts[index] is also larger than start, if so it's before the ledstrip
        if (_lightStripDistanceStarts[indexStart] > start)
        {
            idStart = _lightStripIdStarts[indexStart];
        }
        else
        {
            //the start is on the strip, so let's find the exact id location
            idStart = FindExactLocation(start, indexStart);
            displayable = true;
        }
        
        //find the first _lightStripDistanceEnds that is greater than stop
        var indexStop = _lightStripDistanceEnds.FindIndex(x => x > end);
        if (indexStop == -1)
        {
            //something went quite wrong, normally we shouldn't ever get here.
            idStop = -1;
            return false;
        }
        
        //check if the _lightStripDistanceStarts[index] is also larger than stop, if so it's before the ledstrip
        if (_lightStripDistanceStarts[indexStop] > end)
        {
            idStop = _lightStripIdEnds[indexStop];
        }
        else
        {
            //the stop is on the strip, so let's find the exact id location
            idStop = FindExactLocation(end, indexStop);
            displayable = true;
        }

        return displayable;
    }

    private float FitWithinRange(float location)
    {
        if (location < 0)
        {
            location = _lightStripDistanceEnds.Last() + (location % _lightStripDistanceEnds.Last());
        }

        //check if start is greater than the last _lightStripDistanceEnds, if so modulo the start by it
        if (location > _lightStripDistanceEnds.Last())
        {
            location = location % _lightStripDistanceEnds.Last();
        }

        return location;
    }
}