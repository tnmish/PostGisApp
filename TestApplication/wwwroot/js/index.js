
//Определяем карту, координаты центра и начальный масштаб
var map = L.map('map').setView([56.8516, 60.6122], 12);
var latlng;

//Добавляем на нашу карту слой OpenStreetMap
L.tileLayer('http://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    maxZoom: 19,
    attribution: '© OpenStreetMap contributors'
}).addTo(map);

let drawnItems = new L.FeatureGroup();
map.addLayer(drawnItems);

// Добавление модалки
function OpenAddModal () {    
    $.get(`Home/AddWarehouse?type=point&lat=${latlng.lat}&lng=${latlng.lng}`, function (data) {
    $('#dialogContent').html(data)
        $('#modDialog').modal('show')
    })
}

map.on('click', function (e) {
    latlng = e.latlng; //Полуаем координаты клика
    console.log(latlng);
    L.marker(latlng).addTo(map);
})

// Получаем список складов
fetch('/Home/GetList')
    .then(response => response.json())
    .then(warehouses => {       

        warehouses.forEach(warehouse => {
            if (warehouse.Geometry) {                
                const geometry = warehouse.Geometry;
                if (geometry.type === 'Point') {
                    L.marker(geometry.coordinates).addTo(map)
                        .bindPopup(`<strong>${warehouse.Name}</strong>
                        <br>Директор: ${warehouse.Director}
                        <br>Адрес: ${warehouse.Address}`);
                } else if (geometry.type === 'Polygon') {
                    L.polygon(geometry.coordinates).addTo(map)
                        .bindPopup(`${warehouse.Name}
                        <br>Директор: ${warehouse.Director}
                        <br>Адрес: ${warehouse.Address}`);
                }
            }
            
        });
    });