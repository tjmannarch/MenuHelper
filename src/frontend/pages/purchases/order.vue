<template>
  <view class="page">
    <wd-toast />

    <view v-if="!generated">
      <!-- 选择食材 -->
      <view class="section-title">选择要采购的食材</view>
      <view class="search-bar">
        <wd-search v-model="keyword" placeholder="搜索食材" />
      </view>
      <wd-cell-group>
        <view v-for="ing in filteredIngredients" :key="ing.id" class="ing-row">
          <wd-checkbox v-model="ing.selected" />
          <view class="ing-info">
            <text class="ing-name">{{ ing.name }}</text>
            <text class="ing-unit">{{ ing.unit }}</text>
          </view>
          <view v-if="ing.selected" class="ing-qty">
            <wd-input
              v-model="ing.qty"
              type="digit"
              placeholder="数量"
              no-border
              input-align="center"
              custom-style="width: 100rpx"
            />
            <text class="ing-unit-label">{{ ing.unit }}</text>
          </view>
        </view>
      </wd-cell-group>

      <view class="btn-area">
        <wd-button
          block type="primary"
          :disabled="selectedCount === 0"
          :loading="generating"
          @click="generate"
        >
          生成采购单（已选 {{ selectedCount }} 种）
        </wd-button>
      </view>
    </view>

    <!-- 生成结果 -->
    <view v-else>
      <view class="section-title">采购单（{{ orderDate }}）</view>

      <view v-if="orderResult.selfPurchaseItems && orderResult.selfPurchaseItems.length > 0" class="order-card">
        <view class="order-card-header">
          <text class="order-card-title">🛒 自购清单</text>
        </view>
        <view v-for="item in orderResult.selfPurchaseItems" :key="item.ingredientId" class="order-item">
          <text class="order-item-name">{{ item.ingredientName }}</text>
          <text class="order-item-qty">{{ item.quantity }}{{ item.unit }}</text>
        </view>
        <view class="order-card-footer">
          <wd-button size="small" plain icon="copy" @click="copyText(orderResult.selfPurchaseText)">复制</wd-button>
        </view>
      </view>

      <view v-for="so in orderResult.supplierOrders" :key="so.supplierId" class="order-card">
        <view class="order-card-header">
          <text class="order-card-title">📦 {{ so.supplierName }}</text>
          <text v-if="so.phone" class="order-card-phone">{{ so.phone }}</text>
        </view>
        <view v-for="item in so.items" :key="item.ingredientId" class="order-item">
          <text class="order-item-name">{{ item.ingredientName }}</text>
          <text class="order-item-qty">{{ item.quantity }}{{ item.unit }}</text>
          <text v-if="item.estimatedUnitPrice" class="order-item-price">≈¥{{ item.estimatedUnitPrice }}</text>
        </view>
        <view class="order-card-footer">
          <wd-button size="small" plain icon="copy" @click="copyText(so.orderText)">复制文本</wd-button>
        </view>
      </view>

      <view class="btn-area">
        <wd-button block plain @click="generated = false">重新选择</wd-button>
      </view>
    </view>
  </view>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useToast } from 'wot-design-uni'
import { api } from '@/utils/api.js'

const toast = useToast()
const keyword = ref('')
const allIngredients = ref([]) // { id, name, unit, defaultUnitPrice, selected, qty }
const generating = ref(false)
const generated = ref(false)
const orderResult = ref(null)

const today = new Date()
const orderDate = `${today.getFullYear()}-${String(today.getMonth() + 1).padStart(2, '0')}-${String(today.getDate()).padStart(2, '0')}`

const filteredIngredients = computed(() => {
  if (!keyword.value) return allIngredients.value
  return allIngredients.value.filter(i => i.name.includes(keyword.value))
})

const selectedCount = computed(() => allIngredients.value.filter(i => i.selected).length)

async function loadIngredients() {
  try {
    const res = await api.get('/api/ingredients', { pageSize: 100 })
    allIngredients.value = (res.items ?? []).map(i => ({
      ...i,
      selected: false,
      qty: '1'
    }))
  } catch (e) {
    toast.error(e.message)
  }
}

async function generate() {
  const selected = allIngredients.value.filter(i => i.selected)
  if (selected.length === 0) return toast.warning('请至少选择一种食材')
  for (const i of selected) {
    if (!parseFloat(i.qty) || parseFloat(i.qty) <= 0)
      return toast.warning(`${i.name} 的数量无效`)
  }
  generating.value = true
  try {
    const payload = {
      items: selected.map(i => ({ ingredientId: i.id, quantity: parseFloat(i.qty) })),
      orderDate
    }
    orderResult.value = await api.post('/api/purchase-orders/generate', payload)
    generated.value = true
  } catch (e) {
    toast.error(e.message)
  } finally {
    generating.value = false
  }
}

function copyText(text) {
  uni.setClipboardData({
    data: text,
    success: () => toast.success('已复制到剪贴板')
  })
}

onMounted(() => {
  uni.setNavigationBarTitle({ title: '生成采购单' })
  loadIngredients()
})
</script>

<style lang="scss" scoped>
.page { background: #f5f5f5; min-height: 100vh; padding-bottom: 60rpx; }
.section-title { font-size: 26rpx; color: #999; padding: 24rpx 32rpx 12rpx; }
.search-bar { background: #fff; padding: 8rpx 16rpx; margin-bottom: 2rpx; }
.ing-row {
  display: flex; align-items: center; padding: 24rpx 32rpx;
  background: #fff; border-bottom: 1rpx solid #f5f5f5; gap: 16rpx;
}
.ing-info { flex: 1; }
.ing-name { font-size: 30rpx; color: #333; display: block; }
.ing-unit { font-size: 22rpx; color: #999; }
.ing-qty {
  display: flex; align-items: center; gap: 8rpx;
  background: #f8f8f8; border-radius: 8rpx; padding: 4rpx 12rpx;
}
.ing-unit-label { font-size: 24rpx; color: #999; }
.btn-area { padding: 32rpx; }
.order-card {
  background: #fff; margin: 16rpx 24rpx; border-radius: 16rpx;
  overflow: hidden; box-shadow: 0 2rpx 12rpx rgba(0,0,0,0.06);
}
.order-card-header { padding: 24rpx 32rpx 16rpx; border-bottom: 1rpx solid #f5f5f5; }
.order-card-title { font-size: 32rpx; font-weight: bold; color: #333; }
.order-card-phone { font-size: 24rpx; color: #999; display: block; margin-top: 4rpx; }
.order-item {
  display: flex; align-items: center; padding: 16rpx 32rpx;
  border-bottom: 1rpx solid #f9f9f9; gap: 16rpx;
}
.order-item-name { flex: 1; font-size: 28rpx; color: #333; }
.order-item-qty { font-size: 28rpx; color: #4a90e2; font-weight: bold; }
.order-item-price { font-size: 24rpx; color: #f5a623; }
.order-card-footer { padding: 16rpx 32rpx; display: flex; justify-content: flex-end; }
</style>
