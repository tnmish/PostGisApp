
    //Определяем карту, координаты центра и начальный масштаб
    var map = L.map('map').setView([56.8516, 60.6122], 12);

    //Добавляем на нашу карту слой OpenStreetMap
    L.tileLayer('http://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        maxZoom: 19,
    attribution: '© OpenStreetMap contributors'
    }).addTo(map);

    let drawnItems = new L.FeatureGroup();
    map.addLayer(drawnItems);

    function OpenAddModal () {
        console.log("cloch")
        $.get('Home/AddWarehouse', function (data) {
        $('#dialogContent').html(data)
            $('#modDialog').modal('show')
        })
}

fetch('/Home/GetList')
    .then(response => response.json())
    .then(warehouses => {
        warehouses.forEach(warehouse => {
            if (warehouse.Geometry) {
                const geometry = JSON.parse(warehouse.Geometry);
                if (geometry.type === 'Point') {
                    L.marker(geometry.coordinates).addTo(map)
                        .bindPopup(`Директор: ${warehouse.Director}<br>Адрес: ${warehouse.Address}`);
                } else if (geometry.type === 'Polygon') {
                    L.polygon(geometry.coordinates).addTo(map)
                        .bindPopup(`Директор: ${warehouse.Director}<br>Адрес: ${warehouse.Address}`);
                }
            }
            
        });
    });