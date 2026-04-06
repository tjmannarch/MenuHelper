<template>
  <view class="page">
    <wd-toast />

    <scroll-view scroll-y class="list-wrap"
      refresher-enabled @refresherrefresh="onRefresh" :refresher-triggered="refreshing">
      <view v-if="loading && alerts.length === 0" class="loading-center">
        <wd-loading size="40px" />
      </view>
      <view v-else-if="alerts.length === 0" class="empty">
        <wd-status-tip image="collect" tip="所有食材均在保质期内 🎉" />
      </view>
      <wd-cell-group v-else>
        <wd-cell
          v-for="item in alerts"
          :key="item.ingredientId"
          :title="item.ingredientName"
        >
          <template #label>
            <text class="last-purchase">上次进货：{{ item.lastPurchaseDate }}（已存放 {{ item.daysStored }} 天）</text>
          </template>
          <template #right-icon>
            <view class="alert-badge" :class="item.daysStored - item.maxShelfDays > 3 ? 'severe' : 'mild'">
              <text class="alert-text">超期 {{ item.daysStored - item.maxShelfDays }} 天</text>
            </view>
          </template>
        </wd-cell>
      </wd-cell-group>
    </scroll-view>
  </view>
</template>

<script setup>
import { ref } from 'vue'
import { onShow } from '@dcloudio/uni-app'
import { useToast } from 'wot-design-uni'
import { api } from '@/utils/api.js'

const toast = useToast()
const alerts = ref([])
const loading = ref(false)
const refreshing = ref(false)

async function loadAlerts() {
  loading.value = true
  try {
    alerts.value = await api.get('/api/freshness-alerts') ?? []
  } catch (e) {
    toast.error(e.message)
  } finally {
    loading.value = false
    refreshing.value = false
  }
}

function onRefresh() { refreshing.value = true; loadAlerts() }

onShow(() => loadAlerts())
</script>

<style lang="scss">
.page { display: flex; flex-direction: column; height: 100vh; background: #f5f5f5; }
.list-wrap { flex: 1; overflow: hidden; }
.loading-center { display: flex; justify-content: center; padding: 100rpx; }
.empty { padding-top: 120rpx; }
.last-purchase { font-size: 24rpx; color: #999; }
.alert-badge {
  padding: 8rpx 16rpx; border-radius: 8rpx;
  &.mild { background: #fff7e6; }
  &.severe { background: #fff0f0; }
}
.alert-text {
  font-size: 24rpx; font-weight: bold;
  .mild & { color: #f5a623; }
  .severe & { color: #ee0a24; }
}
</style>
