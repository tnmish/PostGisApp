
//Определяем карту, координаты центра и начальный масштаб
var map = L.map('map').setView([56.8516, 60.6122], 12);
var latlng;

//Добавляем на нашу карту слой OpenStreetMap
L.tileLayer('http://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    maxZoom: 19,
    attribution: '© OpenStreetMap contributors'
}).addTo(map);

var drawnItems = new L.FeatureGroup();
var loadedItems = new L.FeatureGroup();
map.addLayer(drawnItems);
map.addLayer(loadedItems);

const drawControlOptions = {
    position: 'topright',
    draw: {
        polygon: true, // Enable polygon drawing
        polyline: false, // Disable polyline drawing
        rectangle: true, // Enable rectangle drawing
        circle: false, // Disable circle drawing
        marker: true, // Enable marker drawing
        circlemarker: false // Disable circle marker drawing
    },
    edit: {
        featureGroup: drawnItems, // Feature group to store drawn items
        remove: true // Enable removal of shapes
    }
};

var drawControl = new L.Control.Draw(drawControlOptions);
map.addControl(drawControl);

// Добавление модалки
function OpenModal () {        
    document.getElementById('popup').style.display = 'block';    
    document.getElementById('hideModal').style.display = 'block';
}

function CloseModal() {
    document.getElementById('popup').style.display = 'none';
    document.getElementById('hideModal').style.display = 'none';
}

map.on('draw:created', function (event) {
    const layer = event.layer; // The created shape (e.g., rectangle, polygon)
    drawnItems.addLayer(layer); // Add the shape to the feature group

    // Optional: Log the type and properties of the shape
    console.log('Shape created:', layer);
    if (layer instanceof L.Rectangle) {
        console.log('Rectangle bounds:', layer.getBounds());
    } else if (layer instanceof L.Polygon) {
        console.log('Polygon latlngs:', layer.getLatLngs());
    } else if (layer instanceof L.Marker) {
        console.log('Marker position:', layer.getLatLng());
    }
});

// Получаем список складов
function getList() {
    drawnItems.clearLayers();
    return fetch('/Home/GetList')
        .then(response => response.json())
        .then(warehouses => {

            warehouses.forEach(warehouse => {
                if (warehouse.Geometry) {
                    const geometry = JSON.parse(warehouse.Geometry);
                    if (geometry.type === 'Point') {
                        L.marker(geometry.coordinates).addTo(loadedItems)
                            .bindPopup(`<strong>${warehouse.Name}</strong>
                        <br>Директор: ${warehouse.Director}
                        <br>Адрес: ${warehouse.Address}`);
                    } else if (geometry.type === 'Polygon') {
                        L.polygon(geometry.coordinates).addTo(loadedItems)
                            .bindPopup(`${warehouse.Name}
                        <br>Директор: ${warehouse.Director}
                        <br>Адрес: ${warehouse.Address}`);
                    }
                }

            });
        });
};

getList();

$('#saveWarehouseBtn').on('click', function () {

    const geometry = drawnItems.toGeoJSON().features[0].geometry;

    //непонятно почему после 100 строчки координаты меняются местамы, костылим их на место
    if (geometry.type == 'Point') {
        var tmp = geometry.coordinates[0];
        geometry.coordinates[0] = geometry.coordinates[1];
        geometry.coordinates[1] = tmp;
    }
    else {
        let newCoordinates = [];
        geometry.coordinates[0].forEach(coordinate => {
            newCoordinates.push([coordinate[1], coordinate[0]]);
        })
        geometry.coordinates[0] = newCoordinates;
    }    

    console.log(geometry);
    // Собираем данные из формы
    const formData = {
        Id: $('#Id').val(),
        Name: $('#Name').val(),
        Director: $('#Director').val(),
        Address: $('#Address').val(),
        ActivityType: $('#ActivityType').val(),
        Geometry: JSON.stringify(geometry)
    };

    // Отправляем AJAX-запрос
    $.ajax({
        url: '/Home/AddWarehouse', // URL контроллера и действия
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(formData),
        success: function (response) {
            if (response.success) {
                alert('Склад успешно добавлен!');
                location.reload(); // Перезагрузка страницы или обновление карты
            } else {
                alert(response.message || 'Ошибка при сохранении!');
            }
        },
        error: function () {
            alert('Произошла ошибка при отправке запроса.');
        }
    });

    getList();
});