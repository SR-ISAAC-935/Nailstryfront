let horasOcupadas = [];
let serviciosSeleccionados = [];

// Cargar horas ocupadas cuando se selecciona una fecha
$('#date_calendar').on('change', async function () {
    const fechaSeleccionada = new Date(this.value);

    if (!fechaSeleccionada || isNaN(fechaSeleccionada)) return;

    try {
        const response = await fetch(`/Home/GetHorasOcupadas?fecha=${fechaSeleccionada.toISOString()}`);
        horasOcupadas = await response.json();
        validarHoraSeleccionada();
    } catch (error) {
        console.error('Error al cargar horas ocupadas:', error);
    }
});

// Validar hora seleccionada
$('#date_calendar').on('input', validarHoraSeleccionada);

function validarHoraSeleccionada() {
    const valorInput = $('#date_calendar').val();
    const estaOcupada = horasOcupadas.includes(valorInput);

    if (estaOcupada) {
        $('#horaNoDisponible').removeClass('d-none');
        $('#sending').prop('disabled', true);
    } else {
        $('#horaNoDisponible').addClass('d-none');
        $('#sending').prop('disabled', false);
    }
}

// Agregar servicio a la lista
$(document).on('click', '.add-service', function () {
    const serviceData = JSON.parse($(this).attr('data-service'));

    // Verificar si el servicio ya está agregado
    const yaExiste = serviciosSeleccionados.some(s => s.Name === serviceData.Name);
    if (yaExiste) {
        Swal.fire({
            icon: 'info',
            title: 'Atención',
            text: 'Este servicio ya está agregado',
            confirmButtonText: 'Ok'
        });
        return;
    }

    serviciosSeleccionados.push(serviceData);
    actualizarListaServicios();
});

// Actualizar la lista visual de servicios
function actualizarListaServicios() {
    $('#listServices').empty();

    let totalDuration = 0;
    let totalPrice = 0;

    serviciosSeleccionados.forEach((servicio, index) => {
        totalDuration += servicio.Duration || 0;
        totalPrice += servicio.Price || 0;

        const li = $('<li></li>')
            .addClass('list-group-item d-flex justify-content-between align-items-center')
            .html(`
                <div>
                    <strong>${servicio.Name}</strong><br>
                    <small>$${servicio.Price ? servicio.Price.toFixed(2) : '0.00'} - ${servicio.Duration || 0} min</small>
                </div>
                <button type="button" class="btn btn-sm btn-danger remove-service" data-index="${index}">
                    <i class="bi bi-x"></i> Eliminar
                </button>
            `);

        $('#listServices').append(li);
    });

    $('#totalDuration').text(totalDuration);
    $('#totalPrice').text(totalPrice.toFixed(2));
}

// Eliminar servicio de la lista
$(document).on('click', '.remove-service', function () {
    const index = $(this).data('index');
    serviciosSeleccionados.splice(index, 1);
    actualizarListaServicios();
});
// Enviar formulario
$('#sending').on('click', async function () {
    const fechaHora = $('#date_calendar').val();

    if (!fechaHora) {
        Swal.fire({
            icon: 'warning',
            title: 'Atención',
            text: 'Debes seleccionar fecha y hora',
            confirmButtonText: 'Ok'
        });
        return;
    }

    if (serviciosSeleccionados.length === 0) {
        Swal.fire({
            icon: 'warning',
            title: 'Atención',
            text: 'Debes seleccionar al menos un servicio',
            confirmButtonText: 'Ok'
        });
        return;
    }

    // 🔥 Construir payload JSON (NO FormData)
    const payload = {
        dateReserved: fechaHora,
        services: serviciosSeleccionados.map(s => ({
            idService: s.IdService,   // 🔴 IMPORTANTE
            quantity: 1
        })),
        notes: ""
    };

    try {
        const response = await fetch('/Home/CrearCita', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(payload)
        });

        const data = await response.json();
        console.log(payload)
        if (data.success) {
            const totalDuration = $('#totalDuration').text();
            const totalPrice = $('#totalPrice').text();

            Swal.fire({
                icon: 'success',
                title: '¡Cita Agendada!',
                html: `
                    <p><strong>Fecha:</strong> ${data.fecha}</p>
                    <p><strong>Duración total:</strong> ${totalDuration} minutos</p>
                    <p><strong>Precio total:</strong> $${totalPrice}</p>
                `,
                confirmButtonText: 'Genial'
            }).then(() => {
                $('#date_calendar').val('');
                serviciosSeleccionados = [];
                actualizarListaServicios();
                location.reload();
            });

        } else {
            Swal.fire({
                icon: 'error',
                title: 'Algo salió mal',
                text: data.message,
                confirmButtonText: 'Ok'
            });
        }

    } catch (error) {
        Swal.fire({
            icon: 'error',
            title: 'Fatal',
            text: 'Ocurrió un error al agendar la cita',
            confirmButtonText: 'Ok'
        });
        console.error('Error:', error);
    }
});


// Cargar y mostrar mis citas
async function cargarMisCitas() {
    try {
        const response = await fetch('/Home/GetMisCitas');
        const data = await response.json();

        if (data.success) {
            mostrarMisCitas(data.citas);
        }
    } catch (error) {
        console.error('Error al cargar citas:', error);
    }
}

function mostrarMisCitas(citas) {
    const container = $('#misCitas');
    container.empty();

    if (citas.length === 0) {
        container.html('<p class="text-muted">No tienes citas agendadas</p>');
        return;
    }

    citas.forEach(cita => {
        const servicios = JSON.parse(cita.Services);
        const fecha = new Date(cita.DateReserved);

        const card = $(`
            <div class="card mb-3">
                <div class="card-body">
                    <h5 class="card-title">${fecha.toLocaleString('es-GT')}</h5>
                    <ul class="list-unstyled">
                        ${servicios.map(s => `<li>• ${s.Name} - $${s.Price || 0}</li>`).join('')}
                    </ul>
                    <button class="btn btn-danger btn-sm cancel-cita" data-id="${cita.IdDate}">
                        Cancelar Cita
                    </button>
                </div>
            </div>
        `);

        container.append(card);
    });
}

// Cancelar cita
$(document).on('click', '.cancel-cita', async function () {
    const citaId = $(this).data('id');

    const result = await Swal.fire({
        title: '¿Estás seguro?',
        text: "¿Deseas cancelar esta cita?",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        confirmButtonText: 'Sí, cancelar',
        cancelButtonText: 'No'
    });

    if (result.isConfirmed) {
        try {
            const formData = new FormData();
            formData.append('id', citaId);

            const response = await fetch('/Home/CancelarCita', {
                method: 'POST',
                body: formData
            });

            const data = await response.json();

            if (data.success) {
                Swal.fire('Cancelada', data.message, 'success');
                cargarMisCitas();
            } else {
                Swal.fire('Error', data.message, 'error');
            }
        } catch (error) {
            Swal.fire('Error', 'Ocurrió un error al cancelar la cita', 'error');
        }
    }
});

// Inicializar al cargar la página
$(document).ready(function () {
    if ($('#misCitas').length) {
        cargarMisCitas();
    }
});