var markers = [];

function initMap() {
    navigator.geolocation.getCurrentPosition(coordenadas);
}//Fin de la funcion initMap.

function coordenadas(position) {
    var directionsService = new google.maps.DirectionsService
    var directionsDisplay = new google.maps.DirectionsRenderer;
    var lat = position.coords.latitude;
    var lon = position.coords.longitude;
    var waypts = [{
        location: {
            lat: lat,
            lng: lon
        },
        stopover: true
    }, {
        location: {
            lat: 10.002672,
            lng: -83.644236
        }, stopover: true
    }];
    var map = new google.maps.Map(document.getElementById('map'),
        {
            zoom: 35,
            mapTypeId: google.maps.MapTypeId.HYBRID,
            center: {
                lat: waypts[0].location.lat,
                lng: waypts[0].location.lng
            }
        });

    var marker = new google.maps.Marker({
        position: { lat: lat, lng: lon },
        map: map,
        title: 'Nuestra ubicación'
    });
    markers.push(marker);

    var marker2 = new google.maps.Marker({
        position: {
            lat: waypts[0].location.lat,
            lng: waypts[0].location.lng
        },
        map: map,
        title: 'Tu ubicación'
    });
    markers.push(marker2);

    directionsDisplay.setMap(map);
    directionsService.route({
        origin: {
            lat: waypts[0].location.lat,
            lng: waypts[0].location.lng
        },
        destination: {
            lat: waypts[0].location.lat,
            lng: waypts[0].location.lng
        },
        waypoints: waypts,
        travelMode: google.maps.TravelMode.WALKING
    },
    function (response, status) {
        if (status === google.maps.DirectionsStatus.OK) {
            directionsDisplay.setDirections(response);
        } else {
            window.alert('Ha fallado la comunicación con el mapa a causa de: ' + status);
        }
    });
}//Fin de la función coordenadas.