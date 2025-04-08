
//Определяем карту, координаты центра и начальный масштаб
var map = L.map('map').setView([56.8516, 60.6122], 12);
var latlng;

//Добавляем на нашу карту слой OpenStreetMap
L.tileLayer('http://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    maxZoom: 19,
    attribution: '© OpenStreetMap contributors'
}).addTo(map);

var drawnItems = new L.FeatureGroup();
map.addLayer(drawnItems);

var drawControl = new L.Control.Draw({
    edit: {
        featureGroup: drawnItems
    }
});
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

map.on('click', function (e) {
    var useShape = $("input:radio[name=radio]:checked").val();

    switch (useShape) {
        case 'marker': {
            latlng = e.latlng; //Полуаем координаты клика   
            const marker = L.marker(latlng);            
            marker.addTo(drawnItems);
            break;
        }
    }
    
})

// Получаем список складов
fetch('/Home/GetList')
    .then(response => response.json())
    .then(warehouses => {       

        warehouses.forEach(warehouse => {
            if (warehouse.Geometry) {                
                const geometry = JSON.parse(warehouse.Geometry);
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

$('#saveWarehouseBtn').on('click', function () {

    const geometry = drawnItems.toGeoJSON().features[0].geometry;
    console.log(drawnItems);
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
});