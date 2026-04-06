<template>
  <view class="page">
    <wd-toast />

    <!-- 时间段选择 -->
    <view class="period-bar">
      <wd-segmented :options="periods" v-model:value="period" @change="onPeriodChange" />
    </view>

    <!-- 概览卡片 -->
    <view class="overview-cards" v-if="overview">
      <view class="card">
        <text class="card-value">¥{{ overview.totalRevenue.toFixed(2) }}</text>
        <text class="card-label">营业额</text>
      </view>
      <view class="card-divider" />
      <view class="card">
        <text class="card-value cost">¥{{ overview.totalPurchaseCost.toFixed(2) }}</text>
        <text class="card-label">采购成本</text>
      </view>
      <view class="card-divider" />
      <view class="card">
        <text class="card-value" :class="overview.grossProfit >= 0 ? 'profit-pos' : 'profit-neg'">
          ¥{{ overview.grossProfit.toFixed(2) }}
        </text>
        <text class="card-label">毛利润</text>
      </view>
    </view>
    <view v-else class="overview-loading">
      <wd-loading size="30px" />
    </view>

    <!-- 明细标签 -->
    <view class="detail-tabs">
      <view
        v-for="tab in detailTabs"
        :key="tab.key"
        class="detail-tab"
        :class="{ active: activeTab === tab.key }"
        @click="activeTab = tab.key; loadDetail()"
      >{{ tab.label }}</view>
    </view>

    <!-- 明细列表 -->
    <scroll-view scroll-y class="detail-list"
      refresher-enabled @refresherrefresh="onRefresh" :refresher-triggered="refreshing">
      <view v-if="detailLoading" class="loading-center">
        <wd-loading size="30px" />
      </view>
      <view v-else-if="detailItems.length === 0" class="empty">
        <wd-status-tip image="search" tip="暂无数据" />
      </view>
      <view v-else>
        <view v-for="(item, idx) in detailItems" :key="idx" class="detail-row">
          <view class="detail-row-info">
            <text class="detail-name">{{ item.name }}</text>
            <text class="detail-sub" v-if="item.sub">{{ item.sub }}</text>
          </view>
          <view class="detail-row-right">
            <text class="detail-amount">¥{{ item.amount.toFixed(2) }}</text>
            <view class="progress-bar">
              <view class="progress-fill" :style="{ width: item.pct + '%' }" />
            </view>
          </view>
        </view>
      </view>
    </scroll-view>
  </view>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { onShow } from '@dcloudio/uni-app'
import { useToast } from 'wot-design-uni'
import { api } from '@/utils/api.js'

const toast = useToast()
const periods = ['今日', '本周', '本月']
const period = ref('今日')
const overview = ref(null)
const detailItems = ref([])
const detailLoading = ref(false)
const refreshing = ref(false)
const activeTab = ref('category')
const detailTabs = [
  { key: 'category', label: '按分类' },
  { key: 'supplier', label: '按供应商' },
  { key: 'ingredient', label: '按食材' }
]

function formatDate(ts) {
  const d = new Date(ts)
  return `${d.getFullYear()}-${String(d.getMonth()+1).padStart(2,'0')}-${String(d.getDate()).padStart(2,'0')}`
}

function getDateRange(p) {
  const today = new Date()
  today.setHours(0,0,0,0)
  const todayTs = today.getTime()
  if (p === '今日') return { from: formatDate(todayTs), to: formatDate(todayTs) }
  if (p === '本周') {
    const day = today.getDay() || 7
    const monday = new Date(today)
    monday.setDate(today.getDate() - (day - 1))
    return { from: formatDate(monday.getTime()), to: formatDate(todayTs) }
  }
  if (p === '本月') {
    const first = new Date(today.getFullYear(), today.getMonth(), 1)
    return { from: formatDate(first.getTime()), to: formatDate(todayTs) }
  }
  return { from: formatDate(todayTs), to: formatDate(todayTs) }
}

async function loadOverview() {
  overview.value = null
  try {
    const range = getDateRange(period.value)
    overview.value = await api.get('/api/finance/overview', range)
  } catch (e) {
    toast.error(e.message)
  }
}

async function loadDetail() {
  detailLoading.value = true
  try {
    const range = getDateRange(period.value)
    let raw = []
    if (activeTab.value === 'category') {
      raw = await api.get('/api/finance/purchase-by-category', range)
      detailItems.value = mapItems(raw, item => item.categoryName, null, item => item.totalAmount)
    } else if (activeTab.value === 'supplier') {
      raw = await api.get('/api/finance/purchase-by-supplier', range)
      detailItems.value = mapItems(raw, item => item.supplierName, null, item => item.totalAmount)
    } else {
      raw = await api.get('/api/finance/purchase-by-ingredient', range)
      detailItems.value = mapItems(raw, item => item.ingredientName, item => `${item.totalQuantity}${item.unit}`, item => item.totalAmount)
    }
  } catch (e) {
    toast.error(e.message)
  } finally {
    detailLoading.value = false
    refreshing.value = false
  }
}

function mapItems(raw, getName, getSub, getAmount) {
  const max = Math.max(...raw.map(x => getAmount(x)), 1)
  return raw.map(x => ({
    name: getName(x),
    sub: getSub ? getSub(x) : null,
    amount: getAmount(x),
    pct: (getAmount(x) / max * 100).toFixed(1)
  }))
}

function onPeriodChange() {
  loadOverview()
  loadDetail()
}

function onRefresh() {
  refreshing.value = true
  loadOverview()
  loadDetail()
}

async function loadAll() {
  await Promise.all([loadOverview(), loadDetail()])
}

onShow(() => loadAll())
onMounted(() => {
  uni.setNavigationBarTitle({ title: '财务统计' })
})
</script>

<style lang="scss">
.page { display: flex; flex-direction: column; height: 100vh; background: #f5f5f5; }
.period-bar { background: #fff; padding: 20rpx 32rpx; border-bottom: 1rpx solid #f0f0f0; display: flex; justify-content: center; }
.overview-cards {
  background: linear-gradient(135deg, #4a90e2, #357abd);
  padding: 32rpx;
  display: flex; align-items: center; justify-content: space-around;
}
.card { display: flex; flex-direction: column; align-items: center; flex: 1; }
.card-value { font-size: 36rpx; font-weight: bold; color: #fff; }
.card-value.cost { color: rgba(255,255,255,0.8); }
.card-value.profit-pos { color: #a8f0c0; }
.card-value.profit-neg { color: #ffb3b3; }
.card-label { font-size: 22rpx; color: rgba(255,255,255,0.7); margin-top: 8rpx; }
.card-divider { width: 1rpx; height: 60rpx; background: rgba(255,255,255,0.3); }
.overview-loading { background: #4a90e2; padding: 48rpx; display: flex; justify-content: center; }
.detail-tabs { display: flex; background: #fff; border-bottom: 1rpx solid #f0f0f0; }
.detail-tab {
  flex: 1; text-align: center; padding: 24rpx 0;
  font-size: 28rpx; color: #666;
  &.active { color: #4a90e2; border-bottom: 4rpx solid #4a90e2; font-weight: bold; }
}
.detail-list { flex: 1; overflow: hidden; }
.loading-center { display: flex; justify-content: center; padding: 60rpx; }
.empty { padding-top: 80rpx; }
.detail-row {
  display: flex; align-items: center; justify-content: space-between;
  padding: 24rpx 32rpx; background: #fff; border-bottom: 1rpx solid #f5f5f5;
}
.detail-row-info { flex: 1; margin-right: 16rpx; }
.detail-name { font-size: 28rpx; color: #333; display: block; }
.detail-sub { font-size: 22rpx; color: #999; margin-top: 4rpx; }
.detail-row-right { display: flex; flex-direction: column; align-items: flex-end; gap: 8rpx; min-width: 160rpx; }
.detail-amount { font-size: 30rpx; font-weight: bold; color: #333; }
.progress-bar { width: 120rpx; height: 8rpx; background: #f0f0f0; border-radius: 4rpx; overflow: hidden; }
.progress-fill { height: 100%; background: #4a90e2; border-radius: 4rpx; }
</style>
