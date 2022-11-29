<template>
  <div>
    <h1 id="invoiceTitle">Create Invoice</h1>
    <hr />

    <div class="invoice-step" v-if="invoiceStep === 1">
      <h2>Step 1: Select Customer</h2>

      <div v-if="customers.length" class="invoice-step-detail">
        <label for="customers">Customer:</label>
        <select
          v-model="selectedCustomerId"
          id="customer"
          class="invoice-customers"
        >
          <option disabled value="">Please select a Customer</option>
          <option v-for="c in customers" :key="c.id" :value="c.id">{{
            c.firstName + ' ' + c.lastName
          }}</option>
        </select>
      </div>
    </div>
    <div class="invoice-step" v-if="invoiceStep === 2">
      <h2>Step 2: Create Order</h2>

      <div v-if="inventory.length" class="invoice-step-detail">
        <label for="product">Product</label>
        <select v-model="newItem.product" class="invoiceLineItem" id="product">
          <option disabled value="">Please select a Product</option>
          <option v-for="i in inventory" :value="i.product" :key="i.product.id">
            {{ i.product.name }}
          </option>
        </select>
        <label for="quantity">Quantity</label>
        <input v-model="newItem.quantity" id="quantity" type="number" min="0" />
      </div>

      <div class="invoice-item-actions">
        <solar-button
          :disabled="!newItem.product || !newItem.quantity"
          @button:click="addLineItem"
          >Add Line Item</solar-button
        >
        <solar-button
          :disabled="!lineItems.length"
          @button:click="finalizeOrder"
          >Finalize Order</solar-button
        >
      </div>

      <div class="invoice-order-list" v-if="lineItems.length">
        <div class="runningTotal">
          <h3>Running Total:</h3>
          {{ runningTotal | price }}
        </div>
        <hr />
      </div>
    </div>
    <div class="invoice-step" v-if="invoiceStep === 3"></div>
    <hr />
    <table class="">
      <thead>
        <tr>
          <th>Product</th>
          <th>Description</th>
          <th>Qty</th>
          <th>Price</th>
          <th>Subtotal</th>
        </tr>
      </thead>
      <tr v-for="lineItem in lineItems" :key="`index_${lineItem.product.id}`">
        <td>{{ lineItem.product.name }}</td>
        <td>{{ lineItem.product.description }}</td>
        <td>{{ lineItem.quantity }}</td>
        <td>{{ lineItem.product.price }}</td>
        <td>{{ (lineItem.product.price * lineItem.quantity) | price }}</td>
      </tr>
    </table>

    <div class="invoice-steps-actions">
      <solar-button @button:click="prev" :disabled="!canGoPrev"
        >Previous</solar-button
      >
      <solar-button @button:click="next" :disabled="!canGoNext"
        >Next</solar-button
      >
      <solar-button @button:click="startOver">Start Over</solar-button>
    </div>
  </div>
</template>

<script lang="ts">
import SolarButton from '@/components/SolarButton.vue';
import CustomerService from '@/services/customer-service';
import { InventoryService } from '@/services/inventory-service';
import InvoiceService from '@/services/invoice-service';
import { ICustomer } from '@/types/Customer';
import { IInvoice, ILineItem } from '@/types/IInvoice';
import { IProductInventory } from '@/types/Product';
import { Component, Vue } from 'vue-property-decorator';

const customerService = new CustomerService();
const inventoryService = new InventoryService();
const invoiceService = new InvoiceService();

@Component({ name: 'CreateInvoice', components: { SolarButton } })
export default class CreateInvoice extends Vue {
  invoiceStep = 1;
  invoice: IInvoice = {
    createdOn: new Date(),
    customerId: 0,
    lineItems: [],
    updatedOn: new Date(),
  };

  customers: ICustomer[] = [];
  selectedCustomerId = 0;
  inventory: IProductInventory[] = [];
  lineItems: ILineItem[] = [];
  newItem: ILineItem = {
    product: undefined,
    quantity: 0,
  };

  // Computed property
  get canGoNext() {
    if (this.invoiceStep === 1) {
      return this.selectedCustomerId !== 0;
    }

    if (this.invoiceStep === 2) {
      return true;
    }

    if (this.invoiceStep == 3) {
      return false;
    }

    return false;
  }

  get runningTotal() {
    return this.lineItems.reduce(
      (a, b) => a + b['product']['price'] * b['quantity'],
      0
    );
  }

  addLineItem() {
    const newItem: ILineItem = {
      product: this.newItem.product,
      //quantity: parseInt(this.newItem.quantity.toString())
      quantity: Number(this.newItem.quantity),
    };

    // Get the product Ids of the existing products
    const existingItems = this.lineItems.map((item) => item.product.id);

    // If the item I am adding is among the existing items....
    // e.g. adding the same thing again...
    // Then, find the mathing product and update it quantity
    if (existingItems.includes(newItem.product.id)) {
      const lineItem = this.lineItems.find(
        (item) => item.product.id === newItem.product.id
      );

      let currentQuantity = Number(lineItem.quantity);
      const updatedQuantity = (currentQuantity += newItem.quantity);
      lineItem.quantity = updatedQuantity;
    } else {
      this.lineItems.push(this.newItem);
    }

    this.newItem = { product: undefined, quantity: 0 };
  }

  startOver(): void {
    this.invoice = { customerId: 0, lineItems: [] };
    this.invoiceStep = 1;
  }

  finalizeOrder() {
    this.invoiceStep = 3;
  }

  canGoPrev() {
    return this.invoiceStep !== 1;
  }

  prev(): void {
    if (this.invoiceStep === 1) {
      return;
    }
    this.invoiceStep -= 1;
  }

  next(): void {
    if (this.invoiceStep === 3) {
      return;
    }
    this.invoiceStep += 1;
  }

  async initialize(): Promise<void> {
    // customerService
    //   .getCustomers()
    //   .then((res) => (this.customers = res))
    //   .catch(() => {
    //     console.log('Error');
    //   }); // create error modal

    this.customers = await customerService.getCustomers();
    this.inventory = await inventoryService.getInventory();
  }

  async created() {
    await this.initialize();
  }
}
</script>

<style scoped lang="scss">
@import '@/scss/global.scss';

.invoice-step-actions {
  display: flex;
  width: 100%;
}
.invoice-step {
}

.invoice-step-detail {
  margin: 1.2rem;
}

.invoice-order-list {
  margin-top: 1.2rem;
  padding: 0.8rem;

  .line-item {
    display: flex;
    border-bottom: 1px dashed #ccc;
    padding: 0.8rem;
  }

  .item-col {
    flex-grow: 1;
  }
}

.invoice-item-actions {
  display: flex;
}

.price-pre-tax {
  font-weight: bold;
}

.price-final {
  font-weight: bold;
  color: $solar-green;
}

.due {
  font-weight: bold;
}

.invoice-header {
  width: 100%;
  margin-bottom: 1.2rem;
}

.invoice-logo {
  padding-top: 1.4rem;
  text-align: center;

  img {
    width: 200px;
  }
}
</style>
