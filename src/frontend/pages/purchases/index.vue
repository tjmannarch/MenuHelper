<template>
  <view class="page">
    <wd-toast />

    <!-- 顶部工具栏 -->
    <view class="toolbar">
      <wd-button size="small" plain icon="wallet" @click="goBalance">供应商对账</wd-button>
      <wd-button size="small" plain icon="list" @click="goOrder">生成采购单</wd-button>
    </view>

    <!-- 过滤标签 -->
    <wd-tabs v-model="filterTab" @change="onTabChange">
      <wd-tab title="全部" :name="'all'" />
      <wd-tab title="未结算" :name="'unpaid'" />
      <wd-tab title="已结算" :name="'paid'" />
    </wd-tabs>

    <scroll-view scroll-y class="list-wrap" @scrolltolower="loadMore"
      refresher-enabled @refresherrefresh="onRefresh" :refresher-triggered="refreshing">
      <view v-if="list.length === 0 && !loading" class="empty">
        <wd-status-tip image="search" tip="暂无进货记录，点击右下角添加" />
      </view>
      <wd-cell-group v-else>
        <wd-cell
          v-for="item in list"
          :key="item.id"
          :title="item.supplierName || '自购'"
          is-link
          @click="goDetail(item.id)"
        >
          <template #label>
            <view class="cell-label">
              <text class="date-text">{{ item.purchaseDate }} · {{ item.itemCount }}种食材</text>
              <wd-tag size="small" :type="item.isPaid ? 'success' : 'warning'">
                {{ item.isPaid ? '已结算' : '未结算' }}
              </wd-tag>
            </view>
          </template>
          <template #right-icon>
            <text :style="{ color: item.isPaid ? '#07c160' : '#f5a623', fontWeight: 'bold', fontSize: '28rpx' }">
              ¥{{ item.totalAmount.toFixed(2) }}
            </text>
          </template>
        </wd-cell>
      </wd-cell-group>
      <view v-if="loading" class="loading-tip">
        <wd-loading size="20px" /><text class="loading-text">加载中...</text>
      </view>
      <view v-if="noMore && list.length > 0" class="no-more">没有更多了</view>
    </scroll-view>

    <view class="fab" @click="goCreate">
      <wd-icon name="add" size="28px" color="#fff" />
    </view>
  </view>
</template>

<script setup>
import { ref } from 'vue'
import { onShow } from '@dcloudio/uni-app'
import { useToast } from 'wot-design-uni'
import { api } from '@/utils/api.js'

const toast = useToast()
const filterTab = ref('all')
const list = ref([])
const loading = ref(false)
const refreshing = ref(false)
const pageIndex = ref(1)
const noMore = ref(false)
const PAGE_SIZE = 20

async function loadList(reset = true) {
  if (reset) { pageIndex.value = 1; noMore.value = false }
  if (noMore.value || loading.value) return
  loading.value = true
  try {
    const params = {
      pageIndex: pageIndex.value,
      pageSize: PAGE_SIZE,
      isPaid: filterTab.value === 'paid' ? true : filterTab.value === 'unpaid' ? false : undefined
    }
    const res = await api.get('/api/purchases', params)
    const items = res.items ?? []
    list.value = reset ? items : [...list.value, ...items]
    if (items.length < PAGE_SIZE) noMore.value = true
  } catch (e) {
    toast.error(e.message)
  } finally {
    loading.value = false
    refreshing.value = false
  }
}

function loadMore() {
  if (!noMore.value && !loading.value) {
    pageIndex.value++
    loadList(false)
  }
}
function onRefresh() { refreshing.value = true; loadList(true) }
function onTabChange({ name }) { filterTab.value = name; loadList(true) }
function goDetail(id) { uni.navigateTo({ url: `/pages/purchases/detail?id=${id}` }) }
function goCreate() { uni.navigateTo({ url: '/pages/purchases/create' }) }
function goBalance() { uni.navigateTo({ url: '/pages/purchases/balance' }) }
function goOrder() { uni.navigateTo({ url: '/pages/purchases/order' }) }

onShow(() => loadList())
</script>

<style lang="scss" scoped>
.page { display: flex; flex-direction: column; height: 100vh; background: #f5f5f5; }
.toolbar { display: flex; gap: 16rpx; padding: 16rpx 24rpx; background: #fff; border-bottom: 1rpx solid #f0f0f0; }
.list-wrap { flex: 1; overflow: hidden; }
.cell-label { display: flex; align-items: center; gap: 12rpx; margin-top: 6rpx; flex-wrap: wrap; }
.date-text { font-size: 24rpx; color: #999; }
.loading-tip { display: flex; align-items: center; justify-content: center; padding: 24rpx; gap: 12rpx; }
.loading-text { font-size: 24rpx; color: #999; }
.no-more { text-align: center; font-size: 24rpx; color: #ccc; padding: 24rpx; }
.empty { padding-top: 120rpx; }
.fab {
  position: fixed; right: 40rpx; bottom: 80rpx;
  width: 100rpx; height: 100rpx; border-radius: 50%;
  background: #4a90e2; display: flex; align-items: center; justify-content: center;
  box-shadow: 0 4rpx 20rpx rgba(74, 144, 226, 0.5);
}
</style>
