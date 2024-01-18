using GlideNGlow.Common.Models;

namespace GlideNGlow.Rendering.Models;

public class LightStripConverter
{
    List<LightstripData> _lightstrips = new List<LightstripData>();
    List<float> _lightStripDistanceStarts = new List<float>();
    List<int> _lightStripIdStarts = new List<int>();
    List<float> _lightStripDistanceEnds = new List<float>();
    List<int> _lightStripIdEnds = new List<int>();
    List<float> _lightStripPixelDensity = new List<float>();
    
    
    public LightStripConverter(List<LightstripData> lightstrips)
    {
        UpdateLightStripData(lightstrips);
    }
    
    public void UpdateLightStripData(List<LightstripData> lightstrips)
    {
        _lightstrips = lightstrips;
        
        _lightStripDistanceStarts.Clear();
        _lightStripDistanceEnds.Clear();
        
        float currentStart = 0;
        int currentId = 0;
        foreach (var lightstrip in _lightstrips)
        {
            currentStart += lightstrip.DistanceFromLast;
            _lightStripDistanceStarts.Add(currentStart);
            _lightStripIdStarts.Add(currentId);
            
            
            currentStart += lightstrip.Length;
            currentId += lightstrip.Leds;
            _lightStripDistanceEnds.Add(currentStart);
            _lightStripIdEnds.Add(currentId);
            
            _lightStripPixelDensity.Add(lightstrip.Length / lightstrip.Leds);
        }
    }
    //function tries to find an id (int) and puts it in the out parameter
    public bool TryConvertToPixelPosition(float distance, out int id)
    {
        //check if distance is smaller than 0, if so modulo the distance by the last _lightStripDistanceEnds
        distance = FitWithinRange(distance);
        
        //find the first _lightStripDistanceEnds that is greater than distance
        int index = _lightStripDistanceEnds.FindIndex(x => x > distance);
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
        float distanceFromStart = distance - _lightStripDistanceStarts[index];
        //get the id from the start of the lightstrip
        int idFromStart = _lightStripIdStarts[index];
        //get the pixel density of the lightstrip
        float pixelDensity = _lightStripPixelDensity[index];
        //get the id of the pixel
        int pixelId = (int)(distanceFromStart / pixelDensity);
        //add the pixel id to the id from the start of the lightstrip
        return idFromStart + pixelId;
    }

    public bool TryConvertToPixelLine(float start, float end, out int idStart, out int idStop)
    {
        start = FitWithinRange(start);
        
        end = FitWithinRange(end);
        bool displayable = false;
        
        
        //find the first _lightStripDistanceEnds that is greater than start
        int indexStart = _lightStripDistanceEnds.FindIndex(x => x > start);
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
        int indexStop = _lightStripDistanceEnds.FindIndex(x => x > end);
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