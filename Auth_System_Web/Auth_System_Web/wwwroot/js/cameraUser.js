document.addEventListener('DOMContentLoaded', () => {
    const video = document.getElementById('video');
    const canvas = document.getElementById('canvas');
    const context = canvas.getContext('2d');
    const imageBase64Input = document.getElementById('imageBase64');
    const captureButton = document.getElementById('captureButton');
    const nextButton = document.getElementById('nextButton');
    const messageDiv = document.getElementById('message');
    const submitButton = document.getElementById('submitButton');
    const retryButton = document.getElementById('retry');
    const videoContainer = document.getElementById('video-container');
    const form = document.querySelector("form");
    const messageFacesDiv = document.getElementById("message-faces");
    let stream;
    let websocket;
    let lightingTimeout;
    let isLightingGood = false;
    let isWaitingForValidation = false; // Agregar estado para evitar cambios rápidos en el botón


    function startCamera() {
        navigator.mediaDevices.getUserMedia({ video: true })
            .then(mediaStream => {
                stream = mediaStream;
                video.srcObject = stream;
                video.style.display = "block";
                videoContainer.style.display = "block";
                canvas.style.display = "none";
                captureButton.style.display = "inline";
                retryButton.style.display = "none";
                setupWebSocket(); // Conectar WebSocket al iniciar la cámara
            })
            .catch(err => {
                console.error("Error al acceder a la cámara: ", err);
                alert("No se pudo acceder a la cámara.");
            });
    }

    function stopCamera() {
        if (stream) {
            stream.getTracks().forEach(track => track.stop());
        }
        closeWebSocket(); // Cerrar WebSocket al detener la cámara
    }



    function setupWebSocket() {
        websocket = new WebSocket("wss://mobilespecauth-ape9hec0bdbfapda.canadacentral-01.azurewebsites.net/ws/calculate_brightness");


        websocket.onopen = () => {
            console.log("WebSocket conectado");
            startLightingCheck(); // Comenzar a enviar datos de video al servidor
        };

        websocket.onmessage = (event) => {
            const response = JSON.parse(event.data);

            // Propiedades del mensaje 
            const message = response.message;
            const facesDetected = response.faces;

            // Mostrar mensaje de estado de iluminación
            if (message === "Brillo bajo") {
                messageDiv.textContent = 'La iluminación es baja, por favor, busca un lugar más luminoso.';
                messageDiv.style.color = "red";
                captureButton.disabled = true; // Deshabilitar el botón si el brillo es bajo
                isLightingGood = false;
                isWaitingForValidation = false; // Asegurarse de que no se esté esperando validación cuando el brillo es bajo
            } else if (message === "Brillo alto" || message === "Brillo adecuado") {
                messageDiv.textContent = 'La iluminación es suficiente para la captura. Por favor espere 3s para validar la estabilidad luminosa.';
                messageDiv.style.color = "green";

                // Verificar si ambos criterios se cumplen: buena iluminación y detección de rostros
                if ((message === "Brillo alto" || message === "Brillo adecuado") && facesDetected) {
                    // Solo habilitar el botón si estamos esperando validación y ambas condiciones se cumplen
                    if (!isWaitingForValidation) {
                        isWaitingForValidation = true;
                        lightingTimeout = setTimeout(() => {
                            captureButton.disabled = false; // Habilitar el botón solo si hay buen brillo y rostros detectados
                            isWaitingForValidation = false; // Liberar la espera después de 3s
                        }, 3000);
                    }
                } else {
                    // Deshabilitar el botón si no se cumplen ambas condiciones
                    if (!isWaitingForValidation) {
                        captureButton.disabled = true; // Deshabilitar si no se cumplen las condiciones
                    }
                }
            }
        };

        websocket.onerror = (error) => {
            console.error("WebSocket error: ", error);
            alert("Error con la conexión del servidor.");
        };

        websocket.onclose = () => {
            console.log("WebSocket desconectado");
        };
    }



    function closeWebSocket() {
        if (websocket) {
            websocket.close();
        }
        clearTimeout(lightingTimeout); // Limpiar tiempo de validación
    }

    function startLightingCheck() {
        const sendInterval = setInterval(() => {
            if (websocket.readyState === WebSocket.OPEN) {
                canvas.width = video.videoWidth;
                canvas.height = video.videoHeight;
                context.drawImage(video, 0, 0, canvas.width, canvas.height);
                const imageDataUrl = canvas.toDataURL('image/jpeg');
                const base64Image = imageDataUrl.split(',')[1];
                websocket.send(base64Image); // Enviar imagen en base64 al servidor
            } else {
                clearInterval(sendInterval);
            }
        }, 500); // Cada 500ms
    }

    captureButton.addEventListener('click', () => {
        stopCamera(); // Detener cámara y WebSocket al capturar
        canvas.width = video.videoWidth;
        canvas.height = video.videoHeight;
        context.drawImage(video, 0, 0, canvas.width, canvas.height);
        const imageDataUrl = canvas.toDataURL('image/png');
        imageBase64Input.value = imageDataUrl.replace(/^data:image\/(png|jpg);base64,/, '');

        if (imageBase64Input.value) {
            alert("Imagen capturada correctamente.");
            submitButton.style.display = "block";
            captureButton.style.display = "none";
            video.style.display = "none";
            canvas.style.display = "block";
            retryButton.style.display = "inline-block";
            messageDiv.style.display = "none";
        } else {
            alert("Error al capturar la imagen.");
        }
    });

    retryButton.addEventListener('click', () => {
        stopCamera();
        startCamera();
        imageBase64Input.value = "";
        submitButton.style.display = "none";
        captureButton.style.display = "inline-block";
        messageDiv.style.display = "block";
    });

    nextButton.addEventListener('click', (e) => {
        // Verificar que todos los campos obligatorios estén llenos
        const formFields = form.querySelectorAll('input[required], select[required], textarea[required]');
        let allFieldsFilled = true;

        formFields.forEach(field => {
            if (!field.value.trim()) {
                allFieldsFilled = false;
                field.style.borderColor = "red"; // Resaltar campo vacío
            } else {
                field.style.borderColor = ""; // Restablecer color si está lleno
            }
        });

        if (!allFieldsFilled) {
            e.preventDefault(); // Prevenir el envío del formulario si hay campos vacíos
            alert("Por favor, completa todos los campos obligatorios antes de continuar.");
        } else {
            startCamera(); // Iniciar cámara solo si todos los campos están completos
            nextButton.style.display = "none"; // Ocultar el botón de siguiente paso
        }
    });

    document.getElementById('userForm').addEventListener('submit', function (e) {
        if (!imageBase64Input.value) {
            e.preventDefault();
            alert("Por favor, capture una imagen antes de enviar el formulario.");
        }
        stopCamera();
    });
});
