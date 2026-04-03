<template>
	<view class="page">
		<!-- 顶部问候区 -->
		<view class="header">
			<view class="header-left">
				<text class="greeting">{{ greeting }}，店主 👋</text>
				<text class="date-info">{{ dateInfo }}</text>
			</view>
			<image class="header-logo" src="/static/logo.png" mode="aspectFit" />
		</view>

		<!-- 提醒横幅 -->
		<wd-notice-bar
			v-if="notices.length"
			:text="notices[0]"
			prefix="warn-bold"
			type="warning"
		/>

		<!-- 核心操作区 -->
		<view class="action-section">
			<wd-button
				block
				size="large"
				type="primary"
				icon="cart"
				custom-class="main-btn"
				@click="openTodayMenu"
			>
				开今日菜单
			</wd-button>
			<view class="action-row">
				<wd-button
					size="medium"
					plain
					icon="edit"
					custom-class="half-btn"
					@click="goInventory"
				>
					库存盘点
				</wd-button>
				<wd-button
					size="medium"
					plain
					icon="add"
					custom-class="half-btn"
					@click="goAddRevenue"
				>
					录入今日营收
				</wd-button>
			</view>
		</view>

		<!-- 今日概览卡片 -->
		<wd-card title="今日概览" type="rectangle" custom-class="overview-card">
			<view class="overview-row">
				<view class="overview-item">
					<text class="overview-value">{{ todayRevenue > 0 ? '¥' + todayRevenue : '--' }}</text>
					<text class="overview-label">营业额</text>
				</view>
				<view class="overview-divider" />
				<view class="overview-item">
					<text class="overview-value">{{ todayCost > 0 ? '¥' + todayCost : '--' }}</text>
					<text class="overview-label">采购成本</text>
				</view>
				<view class="overview-divider" />
				<view class="overview-item">
					<text class="overview-value" :class="profitClass">
						{{ todayRevenue > 0 ? '¥' + (todayRevenue - todayCost) : '--' }}
					</text>
					<text class="overview-label">毛利润</text>
				</view>
			</view>
		</wd-card>

		<!-- 功能导航宫格 -->
		<view class="section-title">功能模块</view>
		<wd-grid :column="4" clickable border>
			<wd-grid-item icon="app" text="原材料" @click="go('ingredients')" />
			<wd-grid-item icon="user" text="供应商" @click="go('suppliers')" />
			<wd-grid-item icon="cart" text="采购记录" @click="go('purchases')" />
			<wd-grid-item icon="history" text="盘点记录" @click="go('inventory-logs')" />
			<wd-grid-item icon="more1" text="更多" @click="go('more')" />
		</wd-grid>
	</view>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'

// 问候语（按时间段）
const hour = new Date().getHours()
const greeting = hour < 11 ? '早上好' : hour < 14 ? '中午好' : hour < 18 ? '下午好' : '晚上好'

// 今日日期信息
const days = ['周日', '周一', '周二', '周三', '周四', '周五', '周六']
const now = new Date()
const dateInfo = ref(
	`${now.getMonth() + 1}月${now.getDate()}日 ${days[now.getDay()]}`
)

// 提醒事项（后续从接口获取）
const notices = ref(['今晚记得盘点库存！'])

// 今日数据（后续从接口获取）
const todayRevenue = ref(0)
const todayCost = ref(0)
const profitClass = computed(() => {
	const profit = todayRevenue.value - todayCost.value
	return profit > 0 ? 'profit-positive' : profit < 0 ? 'profit-negative' : ''
})

// 跳转方法
const ROUTES = {
  ingredients: '/pages/ingredients/index',
  suppliers: '',
  purchases: '',
  'inventory-logs': '',
  more: '/pages/more/index'
}

const openTodayMenu = () => uni.showToast({ title: '功能开发中', icon: 'none' })
const goInventory = () => uni.showToast({ title: '功能开发中', icon: 'none' })
const goAddRevenue = () => uni.showToast({ title: '功能开发中', icon: 'none' })
const go = (page) => {
  const url = ROUTES[page]
  if (url) uni.navigateTo({ url })
  else uni.showToast({ title: '功能开发中', icon: 'none' })
}
</script>

<style lang="scss">
.page {
	background-color: #f5f5f5;
	min-height: 100vh;
	padding-bottom: 60rpx;
}

/* 顶部问候 */
.header {
	display: flex;
	align-items: center;
	justify-content: space-between;
	padding: 48rpx 32rpx 32rpx;
	background: linear-gradient(135deg, #4a90e2 0%, #357abd 100%);

	.header-left {
		display: flex;
		flex-direction: column;
	}

	.greeting {
		font-size: 36rpx;
		font-weight: bold;
		color: #fff;
	}

	.date-info {
		font-size: 24rpx;
		color: rgba(255, 255, 255, 0.8);
		margin-top: 8rpx;
	}

	.header-logo {
		width: 80rpx;
		height: 80rpx;
		border-radius: 16rpx;
		opacity: 0.9;
	}
}

/* 核心操作区 */
.action-section {
	background-color: #fff;
	padding: 24rpx 32rpx;
	margin-bottom: 16rpx;

	:deep(.main-btn) {
		margin-bottom: 20rpx;
	}

	.action-row {
		display: flex;
		gap: 16rpx;

		:deep(.half-btn) {
			flex: 1;
		}
	}
}

/* 今日概览卡片 */
:deep(.overview-card) {
	margin: 0 24rpx 16rpx;
}

.overview-row {
	display: flex;
	align-items: center;
	justify-content: space-around;
	padding: 8rpx 0;
}

.overview-item {
	display: flex;
	flex-direction: column;
	align-items: center;
	flex: 1;

	.overview-value {
		font-size: 36rpx;
		font-weight: bold;
		color: #333;
	}

	.overview-label {
		font-size: 22rpx;
		color: #999;
		margin-top: 8rpx;
	}
}

.overview-divider {
	width: 1rpx;
	height: 60rpx;
	background-color: #eee;
}

.profit-positive { color: #07c160 !important; }
.profit-negative { color: #ee0a24 !important; }

/* 功能宫格 */
.section-title {
	font-size: 26rpx;
	color: #999;
	padding: 24rpx 32rpx 16rpx;
}
</style>
