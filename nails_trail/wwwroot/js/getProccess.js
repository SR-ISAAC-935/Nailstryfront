const loadedproducts = []; 
const carrito = [];
var datajson = []
const getProductfetch = async () => {
    Swal.fire({
        title: 'Cargando productos...',
        allowOutsideClick: false,
        didOpen: async () => {
            Swal.showLoading();

            try {
                const response = await fetch(
                    'https://localhost:7266/api/Product/GetProducts',
                    { credentials: 'include' }
                );

                if (!response.ok)
                    throw 'Error al obtener productos';

                datajson = await response.json();
                loadedproducts.push(...datajson);
                
                ProductUI.renderCards(datajson);
                Swal.close();
				Swal.fire('Éxito', 'Productos cargados', 'success');
            }
            catch (err) {
                Swal.close()
				Swal.fire('Error', 'no se pudieron cargar los productos', 'error');


            }
        }
    })
};
const ProductUI = {

    renderCards(products) {

        let container = $('#productCards');

        container.empty();

        products.forEach(p => {

            container.append(`

                <div class="card" style="width: 18rem;">
                
                 
                    <img src="${p.product_url_image}"
                         class="card-img-top">

                    <div class="card-body">

                        <h5 class="card-title">
                            ${p.product_name}
                        </h5>

                        <p class="card-text">
                            Q${p.sells_at}
                        </p>

                            <button
       class="btn btn-primary addCart"
      data-id="${p.id_product}">
         Agregar al carrito
            </button>

                    </div>
                </div>
            `);
        });
    }
};
$('#searchInput').on('input', function() {
	showloaded();

})
const showloaded = () => {

    if (!loadedproducts?.length) return;

    const buscaminas = document
        .getElementById('searchInput')
        .value
        .trim()
        .toLowerCase();

    if (buscaminas.length >= 2) {

        const filtered = loadedproducts.filter(p =>
            p.product_name.toLowerCase().includes(buscaminas)
        );

        ProductUI.renderCards(filtered);

    } else {

        ProductUI.renderCards(loadedproducts);

    }
};
$('#productCards').on('click', '.addCart', function () {

    const id = Number($(this).data('id'));

    const selected = datajson.find(p => p.id_product === id);

    if (!selected) {
        console.error('Producto no encontrado:', id);
        return;
    }

    carrito.push({
        id: selected.id_product,
        name: selected.product_name,
        price: selected.sells_at,
        qty: 1
    });

    console.log(carrito);

});
showloaded();
getProductfetch();
