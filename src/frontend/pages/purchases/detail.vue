<template>
  <view class="page">
    <wd-toast />
    <view v-if="loading" class="loading-full">
      <wd-loading size="40px" />
    </view>
    <view v-else-if="detail">
      <!-- 头部信息卡 -->
      <view class="header-card">
        <view class="header-row">
          <text class="supplier-name">{{ detail.supplierName || '自购' }}</text>
          <wd-tag :type="detail.isPaid ? 'success' : 'warning'" size="medium">
            {{ detail.isPaid ? '已结算' : '未结算' }}
          </wd-tag>
        </view>
        <text class="purchase-date">{{ detail.purchaseDate }}</text>
        <text class="total-amount">合计：¥{{ detail.totalAmount.toFixed(2) }}</text>
        <text v-if="detail.remark" class="remark-text">备注：{{ detail.remark }}</text>
      </view>

      <!-- 食材明细 -->
      <view class="section-title">食材明细（{{ detail.items.length }}种）</view>
      <wd-cell-group>
        <wd-cell v-for="item in detail.items" :key="item.id" :title="item.ingredientName">
          <template #label>
            <text class="item-qty">{{ item.quantity }}{{ item.unit }} × ¥{{ item.unitPrice }}/{{ item.unit }}</text>
          </template>
          <template #right-icon>
            <text class="item-total">¥{{ item.totalPrice.toFixed(2) }}</text>
          </template>
        </wd-cell>
      </wd-cell-group>

      <!-- 结算按钮 -->
      <view v-if="!detail.isPaid" class="btn-area">
        <wd-button block type="primary" :loading="paying" @click="pay">标记已结算</wd-button>
      </view>
    </view>
  </view>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useToast } from 'wot-design-uni'
import { api } from '@/utils/api.js'

const toast = useToast()
const id = ref('')
const detail = ref(null)
const loading = ref(false)
const paying = ref(false)

async function loadDetail() {
  loading.value = true
  try {
    detail.value = await api.get(`/api/purchases/${id.value}`)
  } catch (e) {
    toast.error(e.message)
  } finally {
    loading.value = false
  }
}

async function pay() {
  uni.showModal({
    title: '确认结算',
    content: '确认将此进货记录标记为已结算？',
    success: async ({ confirm }) => {
      if (!confirm) return
      paying.value = true
      try {
        await api.post(`/api/purchases/${id.value}/pay`, {})
        toast.success('已标记结算')
        detail.value = { ...detail.value, isPaid: true }
      } catch (e) {
        toast.error(e.message)
      } finally {
        paying.value = false
      }
    }
  })
}

onMounted(() => {
  const pages = getCurrentPages()
  id.value = pages[pages.length - 1].$page?.options?.id || ''
  uni.setNavigationBarTitle({ title: '进货详情' })
  loadDetail()
})
</script>

<style lang="scss" scoped>
.page { background: #f5f5f5; min-height: 100vh; padding-bottom: 60rpx; }
.loading-full { display: flex; justify-content: center; align-items: center; height: 100vh; }
.header-card { background: #fff; padding: 32rpx; margin-bottom: 16rpx; }
.header-row { display: flex; justify-content: space-between; align-items: center; margin-bottom: 12rpx; }
.supplier-name { font-size: 36rpx; font-weight: bold; color: #333; }
.purchase-date { font-size: 26rpx; color: #999; margin-bottom: 8rpx; display: block; }
.total-amount { font-size: 40rpx; font-weight: bold; color: #ee0a24; margin-top: 8rpx; display: block; }
.remark-text { font-size: 24rpx; color: #999; margin-top: 12rpx; display: block; }
.section-title { font-size: 26rpx; color: #999; padding: 24rpx 32rpx 12rpx; }
.item-qty { font-size: 24rpx; color: #999; }
.item-total { font-size: 28rpx; font-weight: bold; color: #333; }
.btn-area { padding: 32rpx; }
</style>
