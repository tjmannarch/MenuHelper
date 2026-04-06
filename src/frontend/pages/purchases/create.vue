<template>
  <view class="page">
    <wd-toast />

    <wd-cell-group custom-class="form-group">
      <!-- 供应商 -->
      <wd-picker
        v-model="form.supplierLabel"
        :columns="supplierColumns"
        label="供应商"
        placeholder="请选择（留空为自购）"
        @confirm="onSupplierConfirm"
      />
      <!-- 进货日期 -->
      <wd-datetime-picker
        v-model="form.purchaseDateTs"
        type="date"
        label="进货日期"
        placeholder="请选择日期"
        @confirm="onDateConfirm"
      />
      <!-- 备注 -->
      <wd-input label="备注" v-model="form.remark" placeholder="选填" clearable />
    </wd-cell-group>

    <!-- 食材明细 -->
    <view class="section-title">食材明细</view>
    <view v-if="items.length === 0" class="items-empty">
      <text class="items-empty-text">还没有添加食材</text>
    </view>
    <wd-cell-group v-else custom-class="items-group">
      <view v-for="(item, idx) in items" :key="idx" class="item-row">
        <view class="item-name-row">
          <text class="item-name">{{ item.ingredientName }}</text>
          <text class="item-unit">{{ item.unit }}</text>
          <wd-icon name="close-bold" size="32rpx" color="#ccc" @click="removeItem(idx)" />
        </view>
        <view class="item-inputs">
          <view class="input-field">
            <text class="input-label">数量</text>
            <wd-input v-model="item.quantity" type="digit" placeholder="0" no-border input-align="right" />
          </view>
          <view class="input-field">
            <text class="input-label">单价(元)</text>
            <wd-input v-model="item.unitPrice" type="digit" placeholder="0.00" no-border input-align="right" />
          </view>
          <text class="item-subtotal">= ¥{{ ((parseFloat(item.quantity) || 0) * (parseFloat(item.unitPrice) || 0)).toFixed(2) }}</text>
        </view>
      </view>
    </wd-cell-group>

    <view class="add-item-btn">
      <wd-button plain icon="add" block @click="showIngredientPicker = true">添加食材</wd-button>
    </view>

    <!-- 合计 -->
    <view class="total-bar" v-if="items.length > 0">
      <text class="total-label">合计金额：</text>
      <text class="total-value">¥{{ totalAmount }}</text>
    </view>

    <view class="btn-area">
      <wd-button block type="primary" :loading="saving" @click="save">保存进货记录</wd-button>
    </view>

    <!-- 食材选择弹出层 -->
    <wd-popup
      v-model="showIngredientPicker"
      position="bottom"
      custom-style="height: 70vh; border-radius: 24rpx 24rpx 0 0; display: flex; flex-direction: column;"
    >
      <view class="popup-header">
        <text class="popup-title">选择食材</text>
        <wd-icon name="close-bold" size="36rpx" @click="showIngredientPicker = false" />
      </view>
      <view class="popup-search">
        <wd-search v-model="ingredientKeyword" placeholder="搜索食材" />
      </view>
      <scroll-view scroll-y style="flex: 1;">
        <wd-cell-group>
          <wd-cell
            v-for="ing in filteredIngredients"
            :key="ing.id"
            :title="ing.name"
            :label="ing.unit"
            :value="ing.defaultUnitPrice ? `¥${ing.defaultUnitPrice}/${ing.unit}` : ''"
            @click="selectIngredient(ing)"
          />
        </wd-cell-group>
      </scroll-view>
    </wd-popup>
  </view>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useToast } from 'wot-design-uni'
import { api } from '@/utils/api.js'

const toast = useToast()
const saving = ref(false)
const showIngredientPicker = ref(false)
const ingredientKeyword = ref('')

const today = new Date()
const todayStr = `${today.getFullYear()}-${String(today.getMonth() + 1).padStart(2, '0')}-${String(today.getDate()).padStart(2, '0')}`

const form = ref({
  supplierLabel: '',
  supplierId: null,
  purchaseDateTs: Date.now(),   // wd-datetime-picker 需要时间戳
  purchaseDateStr: todayStr,    // 提交 API 用的字符串
  remark: ''
})

function onDateConfirm({ value }) {
  const d = new Date(value)
  form.value.purchaseDateStr = `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`
}

const items = ref([]) // { ingredientId, ingredientName, unit, quantity, unitPrice }

const allSuppliers = ref([])
const supplierColumns = computed(() => ['自购', ...allSuppliers.value.map(s => s.name)])

function onSupplierConfirm({ value }) {
  if (value === '自购') {
    form.value.supplierId = null
  } else {
    const found = allSuppliers.value.find(s => s.name === value)
    form.value.supplierId = found?.id ?? null
  }
  form.value.supplierLabel = value
}

const allIngredients = ref([])
const filteredIngredients = computed(() => {
  if (!ingredientKeyword.value) return allIngredients.value
  return allIngredients.value.filter(i => i.name.includes(ingredientKeyword.value))
})

function selectIngredient(ing) {
  if (items.value.find(i => i.ingredientId === ing.id)) {
    toast.warning('该食材已添加')
    return
  }
  items.value.push({
    ingredientId: ing.id,
    ingredientName: ing.name,
    unit: ing.unit,
    quantity: '1',
    unitPrice: ing.defaultUnitPrice ? String(ing.defaultUnitPrice) : '0'
  })
  showIngredientPicker.value = false
  ingredientKeyword.value = ''
}

function removeItem(idx) {
  items.value.splice(idx, 1)
}

const totalAmount = computed(() => {
  return items.value
    .reduce((sum, i) => sum + (parseFloat(i.quantity) || 0) * (parseFloat(i.unitPrice) || 0), 0)
    .toFixed(2)
})

async function loadData() {
  try {
    const [suppRes, ingRes] = await Promise.all([
      api.get('/api/suppliers', { pageSize: 100 }),
      api.get('/api/ingredients', { pageSize: 100 })
    ])
    allSuppliers.value = suppRes.items ?? []
    allIngredients.value = ingRes.items ?? []
  } catch (e) {
    toast.error(e.message)
  }
}

async function save() {
  if (items.value.length === 0) return toast.warning('请至少添加一种食材')
  for (const item of items.value) {
    if (!parseFloat(item.quantity) || parseFloat(item.quantity) <= 0)
      return toast.warning(`${item.ingredientName} 的数量无效`)
    if (parseFloat(item.unitPrice) < 0)
      return toast.warning(`${item.ingredientName} 的单价不能为负数`)
  }

  saving.value = true
  try {
    const payload = {
      supplierId: form.value.supplierId,
      purchaseDate: form.value.purchaseDateStr,
      items: items.value.map(i => ({
        ingredientId: i.ingredientId,
        quantity: parseFloat(i.quantity),
        unitPrice: parseFloat(i.unitPrice)
      })),
      remark: form.value.remark.trim() || null
    }
    await api.post('/api/purchases', payload)
    toast.success('保存成功')
    setTimeout(() => uni.navigateBack(), 1000)
  } catch (e) {
    toast.error(e.message)
  } finally {
    saving.value = false
  }
}

onMounted(() => {
  uni.setNavigationBarTitle({ title: '新增进货记录' })
  loadData()
})
</script>

<style lang="scss" scoped>
.page { background: #f5f5f5; min-height: 100vh; padding-bottom: 60rpx; }
:deep(.form-group) { margin-bottom: 16rpx; }
.section-title { font-size: 26rpx; color: #999; padding: 24rpx 32rpx 12rpx; }
.items-empty { background: #fff; padding: 40rpx; text-align: center; margin-bottom: 16rpx; }
.items-empty-text { font-size: 28rpx; color: #ccc; }
:deep(.items-group) { margin-bottom: 0; }
.item-row { padding: 16rpx 32rpx; background: #fff; border-bottom: 1rpx solid #f5f5f5; }
.item-name-row { display: flex; align-items: center; margin-bottom: 12rpx; }
.item-name { flex: 1; font-size: 30rpx; font-weight: bold; color: #333; }
.item-unit { font-size: 24rpx; color: #999; margin-right: 20rpx; }
.item-inputs { display: flex; align-items: center; gap: 16rpx; }
.input-field {
  display: flex; align-items: center; gap: 8rpx; flex: 1;
  background: #f8f8f8; border-radius: 8rpx; padding: 8rpx 16rpx;
}
.input-label { font-size: 24rpx; color: #999; white-space: nowrap; }
.item-subtotal { font-size: 26rpx; color: #f5a623; font-weight: bold; white-space: nowrap; }
.add-item-btn { padding: 16rpx 32rpx; }
.total-bar {
  background: #fff; padding: 24rpx 32rpx;
  display: flex; justify-content: flex-end; align-items: center; margin-bottom: 16rpx;
}
.total-label { font-size: 28rpx; color: #666; }
.total-value { font-size: 40rpx; font-weight: bold; color: #ee0a24; }
.btn-area { padding: 0 32rpx 32rpx; }
.popup-header {
  display: flex; justify-content: space-between; align-items: center;
  padding: 32rpx 32rpx 16rpx; border-bottom: 1rpx solid #f0f0f0;
}
.popup-title { font-size: 32rpx; font-weight: bold; color: #333; }
.popup-search { padding: 16rpx; background: #fff; }
</style>
