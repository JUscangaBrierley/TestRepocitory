function InitializeVisitMap(visitMapConfig) {
	var myOptions = {
		zoom: visitMapConfig.mapZoom,
		center: visitMapConfig.centerLocation,
		mapTypeId: visitMapConfig.mapTypeId
	};
	var map = new google.maps.Map(document.getElementById(visitMapConfig.mapDivID), myOptions);
	var infowindow = new google.maps.InfoWindow();

	google.maps.event.addListener(map, 'click', function(event) {
		if (infowindow) {
			infowindow.close();
		};
	});

	if (visitMapConfig.storeLocations.length > 0) {
		$.each(visitMapConfig.storeLocations, function () {
			var latlng = new google.maps.LatLng(this.Latitude, this.Longitude);

			var icon = null;
			var markerURL = getMarkerUrlForStore(this);
			if (markerURL && markerURL != '') {
				icon = new google.maps.MarkerImage(markerURL);
			}

			var marker = null;
			if (icon) {
				marker = new google.maps.Marker({ position: latlng, title: this.StoreName, map: map, icon: icon });
			} else {
				marker = new google.maps.Marker({ position: latlng, title: this.StoreName, map: map });
			}

			var infowindowContent = GetStoreLocationInfoWindowContent(this);
			google.maps.event.addListener(marker, 'click', function () {
				if (infowindow) {
					infowindow.close();
				}
				infowindow = new google.maps.InfoWindow({ content: infowindowContent });
				infowindow.open(map, marker);
			});

			this.marker = marker;
		});
	}
}

function getMarkerUrlForStore(store) {
	// If the marker was different by store we'd do something here
	return _visitMapConfig.markerURL;
}

function AddIfNotNull(field, newline) {
    var result = '';
	if (field) {
		if (field != '') {
			if (newline) result += '<br/>'; 
			result += field;
		}
	}
	return result;
}

function GetStoreLocationInfoWindowContent(store) {    
	var storeAddress = AddIfNotNull(store.AddressLineOne, false);
	storeAddress += AddIfNotNull(store.AddressLineTwo, true);
	storeAddress +=	'<br/>' + AddIfNotNull(store.City, false) 
		+ ', ' + AddIfNotNull(store.StateOrProvince, false) 
		+ '   ' + AddIfNotNull(store.ZipOrPostalCode, false);

	var contentString = 
		'<div class="mapInfo">' +
		'   <div class="mapLocationInfoContainer">' +
		'      <div class="mapLocationInfo"><br/>' +
		'         <div class="mapLocationName">' + store.StoreName + '</div>' +
		'         <div class="mapLocationAddress">' + storeAddress + '</div>' +
		'         <div class="mapLocationPhone">' + store.PhoneNumber + '</div>' +
		'      </div>' +
		'      <div class="clear"></div>' +
		'   </div>' +
		'</div>';

	return contentString;
}
