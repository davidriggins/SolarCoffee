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
          class="invoiceCustomers"
        >
          <option disabled value="">Please select a Customer</option>
          <option v-for="c in customers" :key="c.id" :value="c.id">{{
            c.firstName + ' ' + c.lastName
          }}</option>
        </select>
      </div>
    </div>
    <div class="invoice-step" v-if="invoiceStep === 2"></div>
    <div class="invoice-step" v-if="invoiceStep === 3"></div>
  </div>
</template>

<script lang="ts">
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

@Component({ name: 'CreateInvoice' })
export default class CreateInvoice extends Vue {
  invoiceStep = 1;
  invoice: IInvoice = {
    createdOn: new Date(),
    customerId: 0,
    lineItems: [],
    updatedOn: new Date(),
  };

  customers: ICustomer[] = [];
  selectedCustomer = 0;
  inventory: IProductInventory[] = [];
  lineItems: ILineItem[] = [];
  newItem: ILineItem = {
    product: undefined,
    quantity: 0,
  };

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

<style scoped lang="scss"></style>
