const ProductValidator = {

    validate(serviciosList) {

        if (serviciosList.length === 0)
            throw "No hay serviciosList";
		console.log(serviciosList);
        for (let p of serviciosList) {

            if (!p.file)
                throw "Falta imagen";

            if (!this.isValidType(p.file))
                throw "Solo JPG o PNG";
        }
    },

    isValidType(file) {
        const types = ['image/jpeg', 'image/png', 'image/jpg'];
        return types.includes(file.type);
    }
};
const ProductManager = {

    regServices: [],
    contador: 0,

    add(serviciosList) {

        serviciosList.id = this.contador++;
        this.regServices.push(serviciosList);
    },

    remove(id) {

        this.regServices = this.regServices.filter(p => p.id !== id);
    },

    getAll() {

        return this.regServices;
    },

    clear() {

        this.regServices = [];
        this.contador = 0;
    }
};
const ProductFormBuilder = {

    build(serviciosList) {

        let fd = new FormData();

        serviciosList.forEach((p, i) => {

            fd.append(`Services[${i}].ServicesName`, p.ServicesName);
            fd.append(`Services[${i}].ServicesUrlImage`, p.file);
            fd.append(`Services[${i}].IdCategory`, p.IdCategory);
            fd.append(`Services[${i}].prices`, p.prices);
            fd.append(`Services[${i}].duration`, p.duration);
        });
        console.log(serviciosList)
        return fd;
    }
};

const ProductApi = {

    upload(formData) {

        return $.ajax({

            url: 'https://localhost:7266/api/Services/CrearServicios',
            type: 'POST',
            data: formData,
            contentType: false,
            processData: false,
            xhrFields: { withCredentials: true }

        });
    }
};
$('#Submit').on('click', async function (e) {

    e.preventDefault();

    Swal.fire({
        title: 'Subiendo serviciosList...',
        allowOutsideClick: false,
        didOpen: async () => {
            Swal.showLoading();
            try {

                let serviciosList = ProductManager.getAll();

                ProductValidator.validate(serviciosList);

                let fd = ProductFormBuilder.build(serviciosList);

                $('#Submit').prop('disabled', true);

                await ProductApi.upload(fd);

                Swal.fire('Éxito', 'Guardado', 'success');

                ProductManager.clear();
                $('#product_table tbody').empty();

            } catch (err) {

                let msg = err.responseText || err.statusText || err.toString();

                Swal.fire('Error', msg, 'error');
            } finally {

                $('#Submit').prop('disabled', false);
            }
        }
    })
})

const ProductUI = {
    
    addRow(serviciosList) {

        $('#product_table tbody').append(`

            <tr data-id="${serviciosList.id}">
                <td>${serviciosList.ServicesName}</td>
                <td>${serviciosList.file}</td>
                <td>${serviciosList.prices}</td>
                <td>${serviciosList.duration}</td>
                <td>
                    <button class="btn-Delete">
                        Eliminar
                    </button>
                </td>
            </tr>

        `);
    },

    clearForm() {

        $('#ServicesName').val("");
        $('#ServicesUrlImage').val("");
        $('#IdCategory').val("");
        $('#prices').val("");
		$('#duration').val("");
    },
};
$('#AddTable').on('click', function () {

    let file = $('#ServicesUrlImage')[0].files[0];

    if (!file) {
        Swal.fire('Error', 'Selecciona imagen', 'error');
        return;
    }

    let serviciosList = {

        ServicesName: $('#ServicesName').val(),
        file: file,
        IdCategory: $('#IdCategory').val(),
        prices: $('#prices').val(),
        duration: $('#duration').val()
    };
	console.log(serviciosList); 
    ProductManager.add(serviciosList);

    ProductUI.addRow(serviciosList);

    ProductUI.clearForm();
});

$('#product_table').on('click', '.btn-Delete', function () {

    let fila = $(this).closest('tr');
    let id = fila.data('id');

    ProductManager.remove(id);

    fila.remove();
});