var map = null;
var points = [];
var shapes = [];
var center = null;
function LoadMap(latitude, longitude, onMapLoaded) {
    map = new VEMap('theMap');
    options = new VEMapOptions();
    options.EnableBirdseye = false;
    // Makes the control bar less obtrusize.
    map.SetDashboardSize(VEDashboardSize.Small);
    if (onMapLoaded != null)
        map.onLoadMap = onMapLoaded;
    if (latitude != null && longitude != null) {
        center = new VELatLong(latitude, longitude);
    }
    map.LoadMap(center, null, null, null, null, null, null, options);
}
function LoadPin(LL, name, description) {
    var shape = new VEShape(VEShapeType.Pushpin, LL);
    //Make a nice Pushpin shape with a title and description
    shape.SetTitle("<span class=\"pinTitle\"> " + escape(name) + "</span>");
    if (description !== undefined) {
        shape.SetDescription("<p class=\"pinDetails\">" +
        escape(description) + "</p>");
    }
    map.AddShape(shape);
    points.push(LL);
    shapes.push(shape);
}
function FindAddressOnMap(where) {
    var numberOfResults = 20;
    var setBestMapView = true;
    var showResults = true;
    map.Find("", where, null, null, null,
    numberOfResults, showResults, true, true,
    setBestMapView, callbackForLocation);
}
function callbackForLocation(layer, resultsArray, places,
hasMore, VEErrorMessage) {
    clearMap();
    if (places == null)
        return;
    //Make a pushpin for each place we find
    $.each(places, function (i, item) {
        var description = "";
        if (item.Description !== undefined) {
            description = item.Description;
        }
        var LL = new VELatLong(item.LatLong.Latitude,
        item.LatLong.Longitude);
        LoadPin(LL, item.Name, description);
    });
    //Make sure all pushpins are visible
    if (points.length > 1) {
        map.SetMapView(points);
    }
    //If we've found exactly one place, that's our address.
    if (points.length === 1) {
        $("#Latitude").val(points[0].Latitude);
        $("#Longitude").val(points[0].Longitude);
    }
}
function clearMap() {
    map.Clear();
    points = [];
    shapes = [];
}


function initialize() {
    var myLatlng = new google.maps.LatLng(24.18061975930, 79.36565089010);
    var myOptions = {
        zoom: 7,
        center: myLatlng,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    }
    map = new google.maps.Map(document.getElementById("theMap"), myOptions);
    // marker refers to a global variable
    marker = new google.maps.Marker({
        position: myLatlng,
        map: map
    }); 
    // if center changed then update lat and lon document objects
    google.maps.event.addListener(map, 'center_changed', function () {
        var location = map.getCenter();
        document.getElementById("lat").innerHTML = location.lat();

        document.getElementById("lon").innerHTML = location.lng();
        // call function to reposition marker location
        placeMarker(location);
    });
    // if zoom changed, then update document object with new info
    google.maps.event.addListener(map, 'zoom_changed', function () {
        zoomLevel = map.getZoom();
        document.getElementById("zoom_level").innerHTML = zoomLevel;
    });
    // double click on the marker changes zoom level
    google.maps.event.addListener(marker, 'dblclick', function () {
        zoomLevel = map.getZoom() + 1;
        if (zoomLevel == 20) {
            zoomLevel = 10;
        }
        document.getElementById("zoom_level").innerHTML = zoomLevel;
        map.setZoom(zoomLevel);
    });

    function placeMarker(location) {
        var clickedLocation = new google.maps.LatLng(location);
        marker.setPosition(location);
    }
}