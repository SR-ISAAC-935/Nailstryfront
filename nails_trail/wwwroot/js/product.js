const ProductValidator = {

    validate(productos) {

        if (productos.length === 0)
            throw "No hay productos";

        for (let p of productos) {

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

    registros: [],
    contador: 0,

    add(producto) {

        producto.id = this.contador++;
        this.registros.push(producto);
    },

    remove(id) {

        this.registros = this.registros.filter(p => p.id !== id);
    },

    getAll() {

        return this.registros;
    },

    clear() {

        this.registros = [];
        this.contador = 0;
    }
};
const ProductFormBuilder = {

    build(productos) {

        let fd = new FormData();

        productos.forEach((p, i) => {

            fd.append(`Stock[${i}].product_name`, p.product_name);
            fd.append(`Stock[${i}].product_url_image`, p.file);
            fd.append(`Stock[${i}].id_category`, p.idprod);
            fd.append(`Stock[${i}].stock`, p.stock);
            fd.append(`Stock[${i}].buyed_at`, p.buyed_at);
            fd.append(`Stock[${i}].sells_at`, p.sells_at);
        });

        return fd;
    }
};

const ProductApi = {

    upload(formData) {

        return $.ajax({

            url: 'https://localhost:7266/api/Product/UploadImages',
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
        title: 'Subiendo productos...',
        allowOutsideClick: false,
        didOpne: async () => {
            Swal.showLoading();
            try {

                let productos = ProductManager.getAll();

                ProductValidator.validate(productos);

                let fd = ProductFormBuilder.build(productos);

                $('#Submit').prop('disabled', true);

                await ProductApi.upload(fd);

                Swal.fire('Éxito', 'Guardado', 'success');

                ProductManager.clear();
                $('#product_table tbody').empty();

            } catch (err) {

                Swal.fire('Error', err, 'error');

            } finally {

                $('#Submit').prop('disabled', false);
            }
        }
    })
})

const ProductUI = {

    addRow(producto) {

        $('#product_table tbody').append(`

            <tr data-id="${producto.id}">
                <td>${producto.product_name}</td>
                <td>
                    <button class="btn-Delete">
                        Eliminar
                    </button>
                </td>
            </tr>

        `);
    },

    clearForm() {

        $('#product_name').val("");
        $('#product_url_image').val("");
        $('#id_category').val("");
    }
};
$('#AddTable').on('click', function () {

    let file = $('#product_url_image')[0].files[0];

    if (!file) {
        Swal.fire('Error', 'Selecciona imagen', 'error');
        return;
    }

    let producto = {

        product_name: $('#product_name').val(),
        file: file,
        idprod: $('#id_category').val(),
        stock: $('#stock').val(),
        buyed_at: $('#buyed_at').val(),
        sells_at: $('#sells_at').val()
    };

    ProductManager.add(producto);

    ProductUI.addRow(producto);

    ProductUI.clearForm();
});

$('#product_table').on('click', '.btn-Delete', function () {

    let fila = $(this).closest('tr');
    let id = fila.data('id');

    ProductManager.remove(id);

    fila.remove();
});