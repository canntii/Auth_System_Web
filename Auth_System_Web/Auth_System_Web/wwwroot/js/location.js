

console.log("Conectando...")

document.getElementById("location").value = address;
// Función para capturar la ubicación del usuario
function getLocation() {
    return new Promise((resolve, reject) => {
        if (!navigator.geolocation) {
            reject('La geolocalización no es compatible con este navegador');
        }
        navigator.geolocation.getCurrentPosition(
            position => resolve({
                latitude: position.coords.latitude,
                longitude: position.coords.longitude
            }),
            error => reject('Error al obtener la ubicación: ' + error.message),
            {
                enableHighAccuracy: true,
                maximumAge: 0,
                timeout: 5000
            }
        );
    });
}

// Función para pedir la dirección o geolocalización y luego enviar el formulario
async function getAddress() {
    try {
        // Primero, obtenemos la ubicación del usuario
        const location = await getLocation();
        const address = `https://www.google.com/maps?q=${location.latitude},${location.longitude}`;
        console.log(address);

        // Asigna la ubicación formateada al campo oculto
        document.getElementById("location").value = address;

        // Luego, enviamos el formulario
        document.getElementById("scheduleForm").submit();  // Ahora el formulario se envía después de actualizar el campo
    } catch (error) {
        alert("Error: " + error);  // Si ocurre algún error, mostramos una alerta
    }
}